// Vị trí: Bookstore.Core/Models/NV4_Order/Interfaces/IOrderState.cs
using Bookstore.Core.Models.NV4_Order;
namespace Bookstore.Core.Models.NV4_Order.Interfaces {
    public interface IOrderState {
        void Proceed(Order order); // Chuyển sang trạng thái tiếp theo
        void Cancel(Order order);  // Hủy đơn hàng ở trạng thái hiện tại
        string GetStatusName();    // Trả về tên trạng thái (ví dụ: "Đang giao")
    }
}