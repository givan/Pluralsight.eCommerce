using eCommerce.ProductCatalog.Model;
using Microsoft.ServiceFabric.Data;
using Microsoft.ServiceFabric.Data.Collections;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace eCommerce.ProductCatalog
{
    class ProductRepository : IProductRepository
    {
        private const string PRODUCTS_COLLECTION = "products";
        private readonly IReliableStateManager _stateManager;

        public ProductRepository(IReliableStateManager stateManager)
        {
            _stateManager = stateManager;
        }

        public async Task AddProduct(Product product)
        {
            IReliableDictionary<Guid, Product> products = await _stateManager.GetOrAddAsync<IReliableDictionary<Guid, Product>>(PRODUCTS_COLLECTION);

            using (ITransaction tx = this._stateManager.CreateTransaction())
            {
                await products.AddOrUpdateAsync(tx, product.Id, product, (id, value) => product);

                // If an exception is thrown before calling CommitAsync, the transaction aborts, all changes are 
                // discarded, and nothing is saved to the secondary replicas.
                await tx.CommitAsync();
            }
        }

        public async Task<IEnumerable<Product>> GetAllProducts()
        {
            List<Product> result = new List<Product>();

            IReliableDictionary<Guid, Product> products = await _stateManager.GetOrAddAsync<IReliableDictionary<Guid, Product>>(PRODUCTS_COLLECTION);

            using (ITransaction tx = _stateManager.CreateTransaction())
            {
                IAsyncEnumerable<KeyValuePair<Guid, Product>> allProducts = await products.CreateEnumerableAsync(tx, EnumerationMode.Ordered);

                using (IAsyncEnumerator<KeyValuePair<Guid, Product>> enumerator = allProducts.GetAsyncEnumerator())
                {
                    while(await enumerator.MoveNextAsync(CancellationToken.None))
                    {
                        KeyValuePair<Guid, Product> current = enumerator.Current;
                        result.Add(current.Value);
                    }
                }
            }

            return result;
        }
    }
}
