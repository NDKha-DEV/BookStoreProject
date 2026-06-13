// Trong file: Bookstore.Web/Modules/NV4_Order/States/DeliveredState.cs
using Bookstore.Core.Interfaces;
using Bookstore.Core.Models;

namespace Bookstore.Web.Modules.NV4_Order.States
{
    public class DeliveredState : IOrderState
    {
        public string GetStatusName() => "Đã giao thành công";

        public void Proceed(Order order) => throw new InvalidOperationException("Đơn hàng đã hoàn thành, không thể chuyển tiếp.");
        public void Cancel(Order order) => throw new InvalidOperationException("Đơn hàng đã giao xong, không thể hủy.");
    }
}