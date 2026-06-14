using Bookstore.Core.Models.NV3_Cart;
namespace Bookstore.Core.Interfaces {
    public interface ICartRepository {
        Cart GetByUserId(int userId);
        void Save(int userId, Cart cart);
    }
}