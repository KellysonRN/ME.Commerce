using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ME.Commerce.Core.Features.ProductListing.Contracts;
using ME.Commerce.Core.Features.ProductListing.Models;

namespace ME.Commerce.Tests.ProductCatalog
{
    public class FakeProductRepository : IProductRepository
    {
        private readonly ConcurrentDictionary<int, Product> _products = new();

        public Task<List<Product>> GetAllProductsAsync()
        {
            return Task.FromResult(_products.Values.ToList());
        }

        public Task<Product> GetProductAsync(int productId)
        {
            Product product = _products[productId];
            return Task.FromResult(product);
        }

        public Task InsertAsync(Product product)
        {
            _products[product.Id] = product;
            return Task.CompletedTask;
        }
    }
}
