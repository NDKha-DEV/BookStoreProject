// Trong file: Bookstore.Web/Modules/NV4_Order/Observers/CustomerPointsObserver.cs
using Bookstore.Core.Interfaces;
using Bookstore.Core.Models;

namespace Bookstore.Web.Modules.NV4_Order.Observers
{
    public class CustomerPointsObserver : IOrderObserver
    {
        public void UpdateOnOrderDelivered(int orderId)
        {
            // Giả lập tìm đơn hàng và cộng điểm cho user
            var order = MockDataStore.Orders.FirstOrDefault(o => o.Id == orderId);
            if (order != null)
            {
                var user = MockDataStore.Users.FirstOrDefault(u => u.Id == order.UserId);
                if (user != null)
                {
                    // Quy đổi: Cứ mỗi 100k đơn hàng được cộng 1 điểm tích lũy
                    int pointsEarned = (int)(order.TotalAmount / 100000);
                    user.LoyaltyPoints += pointsEarned;
                    
                    Console.WriteLine($"[OBSERVER LOG] Đơn hàng #{orderId} thành công. Khách hàng {user.Username} được cộng {pointsEarned} điểm.");
                }
            }
        }
    }
}