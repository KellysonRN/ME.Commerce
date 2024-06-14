using ME.Commerce.Core.Features.ProductListing.Models;

namespace ME.Commerce.Core.Features.ShoppingCart.Models
{
    public class CartItem
    {
        public required Product Product { get; set; }

        public int Quantity { get; set; }
    }
}
