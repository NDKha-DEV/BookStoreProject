// vị trí: Bookstore.Web/Services/ICartManager.cs
using Bookstore.Web.Models;

namespace Bookstore.Web.Services
{
    public interface ICartManager
    {
        CartDto GetCart();
        void SaveCart(CartDto cart);
        CartDto AddToCart(int productId, int quantity, int? variantId = null);
        CartDto UpdateQuantity(int productId, int quantity, int? variantId = null);
        CartDto RemoveItem(int productId, int? variantId = null);
    }
}
