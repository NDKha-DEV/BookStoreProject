// Trong file: Bookstore.Web/Modules/NV4_Order/States/CancelledState.cs
using Bookstore.Core.Interfaces;
using Bookstore.Core.Models;

namespace Bookstore.Web.Modules.NV4_Order.States
{
    public class CancelledState : IOrderState
    {
        public string GetStatusName() => "Đã hủy";

        public void Proceed(Order order) => throw new InvalidOperationException("Đơn hàng đã bị hủy, không thể tiếp tục xử lý.");
        public void Cancel(Order order) => throw new InvalidOperationException("Đơn hàng này đã được hủy trước đó.");
    }
}