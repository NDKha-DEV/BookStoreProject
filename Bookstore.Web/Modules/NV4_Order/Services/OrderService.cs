// Vị trí: Bookstore.Web/Modules/NV4_Order/Services/OrderService.cs
using System;
using System.Collections.Generic;
using System.Linq;
using Bookstore.Core.Models;
using Bookstore.Core.Models.NV4_Order;
using Bookstore.Core.Models.NV4_Order.Interfaces;
using Bookstore.Core.Models.NV3_Cart;
using Bookstore.Web.Modules.NV1_Account.Services;
using Bookstore.Web.Modules.NV4_Order.States;
using Bookstore.Web.Modules.NV4_Order.Observers;

namespace Bookstore.Web.Modules.NV4_Order.Services
{
    public class OrderService : IOrderService
    {
        public Order CreateOrder(decimal dynamicTotalAmount)
        {
            var authUser = AuthService.Instance.CurrentLoggedInUser; 
            int currentUserId;

            if (authUser != null)
            {
                currentUserId = authUser.Id;
            }
            else
            {
                // Đồng bộ cơ chế lấy User mới nhất để map dữ liệu chuẩn với Controller trong môi trường test
                var lastUser = MockDataStore.Users.LastOrDefault();
                currentUserId = lastUser != null ? lastUser.Id : 3;
            }

            if (!MockDataStore.UserCarts.ContainsKey(currentUserId))
            {
                MockDataStore.UserCarts[currentUserId] = new Cart();
            }
            var currentCart = MockDataStore.UserCarts[currentUserId];

            var newOrder = new Order
            {
                Id = MockDataStore.Orders.Count + 1,
                UserId = currentUserId,
                TotalAmount = dynamicTotalAmount,
                CreatedDate = DateTime.Now,
                PaymentMethod = "PENDING",
                OrderItems = new List<CartItem>(currentCart.Items) 
            };

            newOrder.RegisterObserver(new CustomerPointsObserver());
            newOrder.RegisterObserver(new InventoryObserver());

            newOrder.PaymentStatus = "Unpaid";
            newOrder.ShippingStatus = "NotShipped";
            newOrder.CurrentState = new AwaitingPaymentState();

            MockDataStore.Orders.Add(newOrder);
            return newOrder;
        }

        public void ChangeOrderStatus(int orderId, string action)
        {
            var order = MockDataStore.Orders.FirstOrDefault(o => o.Id == orderId);
            if (order == null) throw new Exception("Không tìm thấy mã đơn hàng cần duyệt!");

            if (action.Equals("proceed", StringComparison.CurrentCultureIgnoreCase))
            {
                if ((order.PaymentMethod == "PENDING" || order.PaymentMethod == "ONLINE") && 
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