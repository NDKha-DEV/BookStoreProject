using Bookstore.Core.Interfaces;
using Bookstore.Core.Models;
using Bookstore.Core.Models.NV3_Cart;

namespace Bookstore.Infrastructure.Repositories {
    public class MockCartRepository : ICartRepository {
        public Cart GetByUserId(int userId) {
            if (!MockDataStore.UserCarts.ContainsKey(userId)) {
                MockDataStore.UserCarts[userId] = new Cart();
            }
            return MockDataStore.UserCarts[userId];
        }
        public void Save(int userId, Cart cart) {
            MockDataStore.UserCarts[userId] = cart;
        }
    }
}