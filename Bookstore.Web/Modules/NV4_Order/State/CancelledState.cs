// Vị trí: Bookstore.Web/Modules/NV4_Order/States/CancelledState.cs
using Bookstore.Core.Models.NV4_Order;
using Bookstore.Core.Models.NV4_Order.Interfaces;

namespace Bookstore.Web.Modules.NV4_Order.States
{
    public class CancelledState : IOrderState
    {
        public string GetStatusName() => "Đơn hàng đã bị hủy bỏ";
        public void Proceed(Order order) => throw new InvalidOperationException("Đơn hàng đã bị hủy, không thể tiếp tục vận hành.");
        public void Cancel(Order order) => throw new InvalidOperationException("Đơn hàng này đã nằm trong trạng thái hủy trước đó.");
    }
}