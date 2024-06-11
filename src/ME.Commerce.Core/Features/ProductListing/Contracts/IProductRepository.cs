using ME.Commerce.Core.Features.ProductListing.Models;

namespace ME.Commerce.Core.Features.ProductListing.Contracts
{
    public interface IProductRepository
    {
        Task<List<Product>> GetAllProductsAsync();

        Task<Product> GetProductAsync(int productId);

        Task InsertAsync(Product product);
    }
}
