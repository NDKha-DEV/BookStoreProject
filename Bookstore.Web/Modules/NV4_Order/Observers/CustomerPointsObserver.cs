// Vị trí: Bookstore.Web/Modules/NV4_Order/Observers/CustomerPointsObserver.cs
using Bookstore.Core.Interfaces;
using Bookstore.Core.Models;

namespace Bookstore.Web.Modules.NV4_Order.Observers
{
    public class CustomerPointsObserver : IOrderObserver
    {
        public void UpdateOnOrderDelivered(int orderId)
        {
            var order = MockDataStore.Orders.FirstOrDefault(o => o.Id == orderId);
            if (order != null && order.ShippingStatus == "Delivered" && order.PaymentStatus == "Paid")
            {
                var user = MockDataStore.Users.FirstOrDefault(u => u.Id == order.UserId);
                if (user != null)
                {
                    int points = (int)(order.TotalAmount / 100000);
                    user.LoyaltyPoints += points;
                    Console.WriteLine($"\n[ OBSERVER] Đơn hàng #{orderId} HOÀN TẤT SONG SONG (Đã Giao & Đã Thu Tiền).");
                    Console.WriteLine($" => Tài khoản '{user.Username}' được cộng thêm {points} điểm tích lũy. (Tổng điểm hiện tại: {user.LoyaltyPoints})");
                }
            }
        }
        public void UpdateOnOrderCancelled(int orderId)
        {
            var order = MockDataStore.Orders.FirstOrDefault(o => o.Id == orderId);
            if (order != null)
            {
                Console.WriteLine($"[ OBSERVER POINTS] Đơn hàng #{orderId} đã bị HỦY. Hệ thống ghi nhận giữ nguyên điểm tích lũy của khách hàng.");
            }
        }
    }
}