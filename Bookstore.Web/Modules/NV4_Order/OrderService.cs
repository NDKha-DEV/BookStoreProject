// Trong file: Bookstore.Web/Modules/NV4_Order/OrderService.cs
using Bookstore.Core.Interfaces;
using Bookstore.Core.Models;
using Bookstore.Web.Modules.NV4_Order.States;
using Bookstore.Web.Modules.NV4_Order.Observers;

namespace Bookstore.Web.Modules.NV4_Order
{
    public class OrderService : IOrderService
    {
        public Order CreateOrderFromCart()
        {
            // Trong thực tế, hàm này sẽ lấy giỏ hàng của NV3 sang để tạo đơn.
            // Ở đây làm mock nhanh một đơn hàng mới để test
            var newOrder = new Order
            {
                Id = MockDataStore.Orders.Count + 1,
                UserId = 2, // Mặc định cho khachhang ở MockDataStore
                TotalAmount = 250000, // Ví dụ đơn giá 250k
                CreatedDate = DateTime.Now
            };

            // Ép trạng thái ban đầu là Chờ Duyệt (State Pattern)
            newOrder.CurrentState = new PendingState();

            // ĐĂNG KÝ CÁC OBSERVER VÀO ĐƠN HÀNG (Observer Pattern)
            newOrder.RegisterObserver(new CustomerPointsObserver());
            newOrder.RegisterObserver(new InventoryObserver());

            // Lưu vào kho RAM dùng chung
            MockDataStore.Orders.Add(newOrder);
            return newOrder;
        }

        public void ChangeOrderStatus(int orderId, string action)
        {
            var order = MockDataStore.Orders.FirstOrDefault(o => o.Id == orderId);
            if (order == null) throw new Exception("Không tìm thấy đơn hàng!");

            // Dựa vào thao tác của Admin để kích hoạt dịch chuyển State tương ứng
            if (action.ToLower() == "proceed")
            {
                order.Proceed(); // Chuyển trạng thái kế tiếp
            }
            else if (action.ToLower() == "cancel")
            {
                order.Cancel(); // Hủy đơn hàng
            }
        }

        public Order GetOrderDetails(int orderId)
        {
            var order = MockDataStore.Orders.FirstOrDefault(o => o.Id == orderId);
            if (order == null) throw new Exception("Không thấy đơn hàng");
            return order;
        }
    }
}