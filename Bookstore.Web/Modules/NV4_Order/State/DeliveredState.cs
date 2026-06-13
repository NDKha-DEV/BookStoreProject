// Vị trí: Bookstore.Web/Modules/NV4_Order/States/DeliveredState.cs
using Bookstore.Core.Interfaces;
using Bookstore.Core.Models;

namespace Bookstore.Web.Modules.NV4_Order.States
{
    public class DeliveredState : IOrderState
    {
        public string GetStatusName() => "Đã hoàn thành toàn diện";
        public void Proceed(Order order) => throw new InvalidOperationException("Đơn hàng đã hoàn thành xong xuôi, không thể xử lý tiếp.");
        public void Cancel(Order order) => throw new InvalidOperationException("Đơn hàng đã giao và thu tiền thành công, không thể hủy.");
    }
}