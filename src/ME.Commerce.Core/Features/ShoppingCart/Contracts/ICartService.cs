using ME.Commerce.Core.Features.ShoppingCart.Models;

namespace ME.Commerce.Core.Features.ShoppingCart.Contracts
{
    public interface ICartService
    {
        void AddToCart(int productId);

        void RemoveFromCart(int productId);

        List<CartItem> GetCartItems();
    }
}
