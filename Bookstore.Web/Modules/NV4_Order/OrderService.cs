// Vị trí: Bookstore.Web/Modules/NV4_Order/OrderService.cs
using Bookstore.Core.Interfaces;
using Bookstore.Core.Models;
using Bookstore.Web.Modules.NV4_Order.States;
using Bookstore.Web.Modules.NV4_Order.Observers;
using Bookstore.Web.Modules.NV1_Account; // ✨ Tối ưu: Đơn giản hóa tên (Name can be simplified)
using System;
using System.Collections.Generic;
using System.Linq;

namespace Bookstore.Web.Modules.NV4_Order
{
    public class OrderService : IOrderService
    {
        public Order CreateOrder(string paymentMethod, decimal dynamicTotalAmount)
        {
            var method = paymentMethod.ToUpper();
            var authUser = AuthService.Instance.CurrentLoggedInUser; // ✨ Đã rút gọn tên nhờ using ở trên
            int currentUserId = authUser != null ? authUser.Id : 3;

            // ✨ Tối ưu: Sử dụng cú pháp Collection Expression [] (Collection initialization can be simplified)
            if (!MockDataStore.UserCarts.ContainsKey(currentUserId))
            {
                MockDataStore.UserCarts[currentUserId] = [];
            }
            var currentCart = MockDataStore.UserCarts[currentUserId];

            var newOrder = new Order
            {
                Id = MockDataStore.Orders.Count + 1,
                UserId = currentUserId,
                TotalAmount = dynamicTotalAmount,
                CreatedDate = DateTime.Now,
                PaymentMethod = method,
                // ✨ Sao chép toàn bộ item từ giỏ hàng thực tế vào đơn hàng
                OrderItems = new List<CartItem>(currentCart) 
            };

            newOrder.RegisterObserver(new CustomerPointsObserver());
            newOrder.RegisterObserver(new InventoryObserver());

            if (method == "ONLINE")
            {
                newOrder.PaymentStatus = "Unpaid";
                newOrder.ShippingStatus = "NotShipped";
                newOrder.CurrentState = new AwaitingPaymentState();
            }
            else // COD
            {
                newOrder.PaymentStatus = "Unpaid";
                newOrder.ShippingStatus = "NotShipped";
                newOrder.CurrentState = new PendingState();
            }

            MockDataStore.Orders.Add(newOrder);
            return newOrder;
        }

        public void ChangeOrderStatus(int orderId, string action)
        {
            var order = MockDataStore.Orders.FirstOrDefault(o => o.Id == orderId);
            if (order == null) throw new Exception("Không tìm thấy mã đơn hàng cần duyệt!");

            if (action.Equals("proceed", StringComparison.CurrentCultureIgnoreCase))
            {
                // Nếu đơn hàng cấu hình thanh toán ONLINE mà người dùng chưa trả tiền (Unpaid) 
                // và đang ở trạng thái chuẩn bị duyệt đi giao (AwaitingPaymentState) -> Chặn đứng ngay!
                if (order.PaymentMethod == "ONLINE" && 
                    order.PaymentStatus.Equals("Unpaid", StringComparison.OrdinalIgnoreCase) && 
                    order.CurrentState is AwaitingPaymentState)
                {
                    throw new Exception("Đơn hàng thanh toán trực tuyến chưa được thanh toán thành công! Không thể chuyển sang trạng thái giao hàng.");
                }

                order.Proceed(); 
            }
            else if (action.Equals("cancel", StringComparison.CurrentCultureIgnoreCase))
            {
                order.Cancel(); 
            }
        }

        public Order GetOrderDetails(int orderId)
        {
            return MockDataStore.Orders.FirstOrDefault(o => o.Id == orderId) 
                   ?? throw new Exception("Không tìm thấy đơn hàng trong kho lưu trữ RAM!");
        }
    }
}