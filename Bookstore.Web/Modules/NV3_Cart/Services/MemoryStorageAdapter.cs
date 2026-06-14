// vị trí: Bookstore.Web/Modules/NV3_Cart/Services/MemoryStorageAdapter.cs
using Bookstore.Core.Interfaces;
using Bookstore.Core.Models;
using Bookstore.Core.Models.NV3_Cart;

namespace Bookstore.Web.Modules.NV3_Cart.Services
{
    // Lớp Adapter đóng vai trò bộ chuyển đổi
    public class MemoryStorageAdapter : ICartStorage
    {
        // Thao tác lấy dữ liệu từ MockDataStore (RAM)
        public bool HasCart(int userId)
        {
            return MockDataStore.UserCarts.ContainsKey(userId);
        }

        public Cart GetCart(int userId)
        {
            return MockDataStore.UserCarts[userId];
        }

        public void SaveCart(int userId, Cart cart)
        {
            MockDataStore.UserCarts[userId] = cart;
        }
    }
}