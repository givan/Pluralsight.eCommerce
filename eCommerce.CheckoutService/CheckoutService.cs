using System;
using System.Collections.Generic;
using System.Fabric;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using eCommerce.CheckoutService.Model;
using eCommerce.ProductCatalog.Model;
using Microsoft.ServiceFabric.Actors;
using Microsoft.ServiceFabric.Actors.Client;
using Microsoft.ServiceFabric.Data;
using Microsoft.ServiceFabric.Data.Collections;
using Microsoft.ServiceFabric.Services.Client;
using Microsoft.ServiceFabric.Services.Communication.Runtime;
using Microsoft.ServiceFabric.Services.Remoting.Client;
using Microsoft.ServiceFabric.Services.Remoting.V2.FabricTransport.Client;
using Microsoft.ServiceFabric.Services.Remoting.V2.FabricTransport.Runtime;
using Microsoft.ServiceFabric.Services.Runtime;
using UserActor.Interfaces;

namespace eCommerce.CheckoutService
{
    /// <summary>
    /// An instance of this class is created for each service replica by the Service Fabric runtime.
    /// </summary>
    internal sealed class CheckoutService : StatefulService, ICheckoutService
    {
        private const string HISTORY_COLLECTION = "history";

        public CheckoutService(StatefulServiceContext context)
            : base(context)
        { }

        public async Task<CheckoutSummary> CheckoutAsync(string userId)
        {
            CheckoutSummary result = new CheckoutSummary()
            {
                Date = DateTime.UtcNow,
                Products = new List<CheckoutProduct>()
            };

            // get user basket
            IUserActor userActor = GetUserActor(userId);
            var basket = await userActor.GetBasket();

            IProductCatalogService catalogService = GetProductCatalogService();

            // build the products using the catalog service data and the basket line items
            foreach (var basketItem in basket)
            {
                Product product = await catalogService.GetProductAsync(basketItem.ProductId);

                if (product != null)
                {
                    var checkoutProduct = new CheckoutProduct()
                    {
                        Product = product,
                        Price = product.Price,
                        Quantity = basketItem.Quantity
                    };

                    result.Products.Add(checkoutProduct);
                }
            }

            // add the current checkout summary to history (for later retrieval)
            await AddToHistoryAsync(result);

            return result;
        }

        public async Task<CheckoutSummary[]> GetOrderHistoryAsync(string userId)
        {
            List<CheckoutSummary> result = new List<CheckoutSummary>();

            IReliableDictionary<DateTime, CheckoutSummary> history =
                await StateManager.GetOrAddAsync<IReliableDictionary<DateTime, CheckoutSummary>>(HISTORY_COLLECTION);

            using (ITransaction tx = StateManager.CreateTransaction())
            {
                IAsyncEnumerable<KeyValuePair<DateTime, CheckoutSummary>> allCheckoutSummaries =
                    await history.CreateEnumerableAsync(tx, EnumerationMode.Unordered);

                using (IAsyncEnumerator<KeyValuePair<DateTime, CheckoutSummary>> enumerator = allCheckoutSummaries.GetAsyncEnumerator())
                {
                    while (await enumerator.MoveNextAsync(CancellationToken.None))
                    {
                        KeyValuePair<DateTime, CheckoutSummary> current = enumerator.Current;

                        result.Add(current.Value);
                    }
                }
            }

            return result.ToArray();
        }

        /// <summary>
        /// Optional override to create listeners (e.g., HTTP, Service Remoting, WCF, etc.) for this service replica to handle client or user requests.
        /// </summary>
        /// <remarks>
        /// For more information on service communication, see https://aka.ms/servicefabricservicecommunication
        /// </remarks>
        /// <returns>A collection of listeners.</returns>
        protected override IEnumerable<ServiceReplicaListener> CreateServiceReplicaListeners()
        {
            return new[]
            {
                new ServiceReplicaListener(context => new FabricTransportServiceRemotingListener(context, this))
            };
        }

        private IProductCatalogService GetProductCatalogService()
        {
            ServiceProxyFactory proxyFactory = new ServiceProxyFactory(c => new FabricTransportServiceRemotingClientFactory());
            return proxyFactory.CreateServiceProxy<IProductCatalogService>(
                new Uri("fabric:/Pluralsight.eCommerce/eCommerce.ProductCatalog"),
                new ServicePartitionKey(0)
                ); ;
        }

        private IUserActor GetUserActor(string userId)
        {
            return ActorProxy.Create<IUserActor>(
                new ActorId(userId),
                new Uri("fabric:/Pluralsight.eCommerce/UserActorService"));
        }

        private async Task AddToHistoryAsync(CheckoutSummary summary)
        {
            IReliableDictionary<DateTime, CheckoutSummary> history = 
                await StateManager.GetOrAddAsync<IReliableDictionary<DateTime, CheckoutSummary>>(HISTORY_COLLECTION);

            using(ITransaction tx = StateManager.CreateTransaction())
            {
                await history.AddAsync(tx, summary.Date, summary);

                await tx.CommitAsync();
            }
        }
    }
}
