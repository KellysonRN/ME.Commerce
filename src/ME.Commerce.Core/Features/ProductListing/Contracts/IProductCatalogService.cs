using ME.Commerce.Core.Features.ProductListing.Models;

namespace ME.Commerce.Core.Features.ProductListing.Contracts
{
    public interface IProductCatalogService
    {
        Task<Product> GetProductAsync(int productId);
    }
}
