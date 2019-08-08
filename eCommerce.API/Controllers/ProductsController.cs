using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using eCommerce.API.Model;
using eCommerce.ProductCatalog.Model;
using Microsoft.AspNetCore.Mvc;
using Microsoft.ServiceFabric.Services.Client;
using Microsoft.ServiceFabric.Services.Remoting.Client;
using Microsoft.ServiceFabric.Services.Remoting.V2.FabricTransport.Client;

namespace eCommerce.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductsController : ControllerBase
    {
        IProductCatalogService _service;

        public ProductsController()
        {
            ServiceProxyFactory proxyFactory = new ServiceProxyFactory(c => new FabricTransportServiceRemotingClientFactory());
            _service = proxyFactory.CreateServiceProxy<IProductCatalogService>(
                new Uri("fabric:/Pluralsight.eCommerce/eCommerce.ProductCatalog"),
                new ServicePartitionKey(0)
                );
        }

        // GET api/products
        [HttpGet]
        public async Task<IEnumerable<ApiProduct>> GetAsync()
        {
            IEnumerable<Product> allProducts = await _service.GetAllProductsAsync();

            return allProducts.Select(p => new ApiProduct
            {
                Id = p.Id,
                Description = p.Description,
                Price = p.Price,
                IsAvailable = p.Availability > 0
            });
        }

        // GET api/products/{productId>
        [HttpGet("{productId}")]
        public async Task<ApiProduct> GetByIdAsync(Guid productId)
        {
            ApiProduct result = new ApiProduct();

            Product product = await _service.GetProductAsync(productId);
            if (product != null)
            {
                result.Id = product.Id;
                result.Description = product.Description;
                result.Price = product.Price;
                result.IsAvailable = product.Availability > 0;
            }

            return result;
        }

        // POST api/products
        [HttpPost]
        public async Task PostAsync([FromBody] ApiProduct product)
        {
            var newProduct = new Product()
            {
                Id = Guid.NewGuid(),
                Description = product.Description,
                Price = product.Price,
                Availability = 100
            };

            await _service.AddProductAsync(newProduct);
        }
    }
}
