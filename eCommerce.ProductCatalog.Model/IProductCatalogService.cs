using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Microsoft.ServiceFabric.Services.Remoting;

namespace eCommerce.ProductCatalog.Model
{
    public interface IProductCatalogService : IService
    {
        Task<Product[]> GetAllProductsAsync();

        Task<Product> GetProductAsync(Guid productId);

        Task AddProductAsync(Product product);
    }
}
