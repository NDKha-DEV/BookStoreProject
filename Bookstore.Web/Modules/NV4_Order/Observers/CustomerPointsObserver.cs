// Vị trí: Bookstore.Web/Modules/NV4_Order/Observers/CustomerPointsObserver.cs
using System;
using System.Linq;
using Bookstore.Core.Models;
using Bookstore.Core.Models.NV4_Order.Interfaces;

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
                    Console.WriteLine($"\n[OBSERVER] Đơn hàng #{orderId} HOÀN TẤT. Tài khoản '{user.Username}' được cộng {points} điểm.");
                }
            }
        }
        public void UpdateOnOrderCancelled(int orderId)
        {
            Console.WriteLine($"[OBSERVER POINTS] Đơn hàng #{orderId} đã bị HỦY. Giữ nguyên điểm tích lũy.");
        }
    }
}