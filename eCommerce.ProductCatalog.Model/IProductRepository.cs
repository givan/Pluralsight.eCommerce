using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace eCommerce.ProductCatalog.Model
{
    public interface IProductRepository
    {
        Task<IEnumerable<Product>> GetAllProducts();

        Task AddProduct(Product product);
    }
}
