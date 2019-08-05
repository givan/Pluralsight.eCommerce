using System;
using System.Collections.Generic;
using System.Fabric;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using eCommerce.ProductCatalog.Model;
using Microsoft.ServiceFabric.Data.Collections;
using Microsoft.ServiceFabric.Services.Communication.Runtime;
using Microsoft.ServiceFabric.Services.Remoting.V2.FabricTransport.Runtime;
using Microsoft.ServiceFabric.Services.Runtime;

namespace eCommerce.ProductCatalog
{
    /// <summary>
    /// An instance of this class is created for each service replica by the Service Fabric runtime.
    /// </summary>
    internal sealed class ProductCatalog : StatefulService, IProductCatalogService
    {
        private ProductRepository _repo;

        public ProductCatalog(StatefulServiceContext context)
            : base(context)
        { }

        public async Task AddProductAsync(Product product)
        {
            await _repo.AddProduct(product);
        }

        public async Task<Product[]> GetAllProductsAsync()
        {
            return (await _repo.GetAllProducts()).ToArray();
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
            return new ServiceReplicaListener[] 
            {
                new ServiceReplicaListener(context => new FabricTransportServiceRemotingListener(context, this))
            };
        }

        /// <summary>
        /// This is the main entry point for your service replica.
        /// This method executes when this replica of your service becomes primary and has write status.
        /// </summary>
        /// <param name="cancellationToken">Canceled when Service Fabric needs to shut down this service replica.</param>
        protected override async Task RunAsync(CancellationToken cancellationToken)
        {
            // TODO: Replace the following sample code with your own logic 
            //       or remove this RunAsync override if it's not needed in your service.

            //var myDictionary = await this.StateManager.GetOrAddAsync<IReliableDictionary<string, long>>("myDictionary");

            //while (true)
            //{
            //    cancellationToken.ThrowIfCancellationRequested();

            //    using (var tx = this.StateManager.CreateTransaction())
            //    {
            //        var result = await myDictionary.TryGetValueAsync(tx, "Counter");

            //        ServiceEventSource.Current.ServiceMessage(this.Context, "Current Counter Value: {0}",
            //            result.HasValue ? result.Value.ToString() : "Value does not exist.");

            //        await myDictionary.AddOrUpdateAsync(tx, "Counter", 0, (key, value) => ++value);

            //        // If an exception is thrown before calling CommitAsync, the transaction aborts, all changes are 
            //        // discarded, and nothing is saved to the secondary replicas.
            //        await tx.CommitAsync();
            //    }

            //    await Task.Delay(TimeSpan.FromSeconds(1), cancellationToken);
            //}

            _repo = new ProductRepository(StateManager);

            Product product1 = new Product
            {
                Id = Guid.NewGuid(),
                Description = "Product 1",
                Price = 10,
                Availability = 100
            };
            await _repo.AddProduct(product1);

            Product product2 = new Product
            {
                Id = Guid.NewGuid(),
                Description = "Product 2",
                Price = 20,
                Availability = 200
            };
            await _repo.AddProduct(product2);

            Product product3 = new Product
            {
                Id = Guid.NewGuid(),
                Description = "Product 3",
                Price = 30,
                Availability = 300
            };
            await _repo.AddProduct(product3);

            Product product4 = new Product
            {
                Id = Guid.NewGuid(),
                Description = "Product 4",
                Price = 40,
                Availability = 400
            };
            await _repo.AddProduct(product4);

            IEnumerable<Product> all = await _repo.GetAllProducts();
        }
    }
}
