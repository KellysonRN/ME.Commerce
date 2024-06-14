using ME.Commerce.Core.Features.ProductListing.Models;

namespace ME.Commerce.Infrastructure
{
    public interface IProductsClient
    {
         Task<Product> GetProductAsync(int productId);
    }
}
