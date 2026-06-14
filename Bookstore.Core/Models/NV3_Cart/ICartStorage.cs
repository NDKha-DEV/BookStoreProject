using Bookstore.Core.Models.NV3_Cart;

namespace Bookstore.Core.Interfaces
{
    public interface ICartStorage
    {
        Cart GetCart(int userId);
        void SaveCart(int userId, Cart cart);
        bool HasCart(int userId);
    }
}