// 4. NV4: IOrderService.cs - Quản lý vòng đời đơn hàng
using Bookstore.Core.Models;
namespace Bookstore.Core.Interfaces {
    public interface IOrderService {
        Order CreateOrderFromCart();
        void ChangeOrderStatus(int orderId, string newStatus); // Admin gọi hàm này
        Order GetOrderDetails(int orderId);
    }
}