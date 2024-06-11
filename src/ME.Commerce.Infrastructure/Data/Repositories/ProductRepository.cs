using ME.Commerce.Core.Features.ProductListing.Contracts;
using ME.Commerce.Core.Features.ProductListing.Models;

namespace ME.Commerce.Infrastructure.Data.Repositories
{
    public class ProductRepository : IProductRepository
    {
        public Task<List<Product>> GetAllProductsAsync()
        {
            throw new NotImplementedException();
        }

        public Task<Product> GetProductAsync(int productId)
        {
            throw new NotImplementedException();
        }

        public Task InsertAsync(Product product)
        {
            throw new NotImplementedException();
        }
    }
}
