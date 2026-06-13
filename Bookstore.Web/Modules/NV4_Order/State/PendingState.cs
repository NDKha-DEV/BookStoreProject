// Trong file: Bookstore.Web/Modules/NV4_Order/States/PendingState.cs
using Bookstore.Core.Interfaces;
using Bookstore.Core.Models;

namespace Bookstore.Web.Modules.NV4_Order.States
{
    public class PendingState : IOrderState
    {
        public string GetStatusName() => "Chờ duyệt";

        public void Proceed(Order order)
        {
            // Từ Chờ duyệt chuyển sang Đang giao
            order.CurrentState = new DeliveringState();
        }

        public void Cancel(Order order)
        {
            // Từ Chờ duyệt có thể Hủy đơn hàng
            order.CurrentState = new CancelledState();
        }
    }
}