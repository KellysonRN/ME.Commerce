using System;
using System.Collections.Concurrent;
using System.Threading.Tasks;
using ME.Commerce.Core.Features.ProductListing.Contracts;
using ME.Commerce.Core.Features.ProductListing.Models;

namespace ME.Commerce.Tests.ProductCatalog
{
    public class FakeProductCatalogService : IProductCatalogService
    {
        private readonly ConcurrentDictionary<int, Product> _products = new();

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
