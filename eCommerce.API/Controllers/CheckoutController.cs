using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using eCommerce.API.Model;
using eCommerce.CheckoutService.Model;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.ServiceFabric.Services.Client;
using Microsoft.ServiceFabric.Services.Remoting.Client;
using Microsoft.ServiceFabric.Services.Remoting.V2.FabricTransport.Client;

namespace eCommerce.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CheckoutController : ControllerBase
    {
        Random rnd = new Random();

        [Route("{userId}")]
        public async Task<ApiCheckoutSummary> CheckoutAsync(string userId)
        {
            CheckoutSummary summary =
                await GetCheckoutService().CheckoutAsync(userId);

            return ToApiCheckoutSummary(summary);
        }

        [Route("history/{userId}")]
        public async Task<ApiCheckoutSummary[]> GetHistoryAsync(string userId)
        {
            CheckoutSummary[] history =
                await GetCheckoutService().GetOrderHistoryAsync(userId);

            return history.Select(ToApiCheckoutSummary).ToArray();
        }

        private ApiCheckoutSummary ToApiCheckoutSummary(CheckoutSummary summary)
        {
            return new ApiCheckoutSummary()
            {
                Products = summary.Products.Select(p => new ApiCheckoutProduct()
                {
                    ProductId = p.Product.Id,
                    ProductDescription = p.Product.Description,
                    Price = p.Product.Price,
                    Quantity = p.Quantity
                }).ToList(),
                Date = summary.Date,
                TotalPrice = summary.TotalPrice
            };
        }

        private ICheckoutService GetCheckoutService()
        {
            long key = LongRandom();

            var proxyFactory = new ServiceProxyFactory(c => new FabricTransportServiceRemotingClientFactory());

            return proxyFactory.CreateServiceProxy<ICheckoutService>(
                new Uri("fabric:/Pluralsight.eCommerce/eCommerce.CheckoutService"),
                new ServicePartitionKey(key)
                );
        }

        private long LongRandom()
        {
            byte[] buf = new byte[8];
            rnd.NextBytes(buf);

            return BitConverter.ToInt64(buf);
        }
    }
}