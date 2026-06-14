// Vị trí: Bookstore.Core/Models/NV4_Order/Interfaces/IOrderService.cs
using Bookstore.Core.Models.NV4_Order;

namespace Bookstore.Core.Models.NV4_Order.Interfaces
{
    public interface IOrderService
    {
        // 🌟 ĐÃ CẬP NHẬT: Thêm tham số dynamicTotalAmount để khớp hoàn toàn với OrderService ở tầng Web
        Order CreateOrder(string paymentMethod, decimal dynamicTotalAmount);
        
        // Điều hướng trạng thái đơn hàng dựa trên hành động ("proceed" hoặc "cancel")
        void ChangeOrderStatus(int orderId, string action);
        
        // Lấy thông tin chi tiết đơn hàng để hiển thị hoặc đối soát dữ liệu
        Order GetOrderDetails(int orderId);
    }
}