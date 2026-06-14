// Vị trí: Bookstore.Web/Modules/NV4_Order/States/AwaitingPaymentState.cs
using Bookstore.Core.Models.NV4_Order;
using Bookstore.Core.Models.NV4_Order.Interfaces;

namespace Bookstore.Web.Modules.NV4_Order.States
{
    public class AwaitingPaymentState : IOrderState
    {
        public string GetStatusName() => "Chờ thanh toán trực tuyến";

        public void Proceed(Order order)
        {
            // Được gọi khi NV5 (Thanh toán) thông báo quét mã thành công
            order.PaymentStatus = "Paid";       // Đổi trạng thái tiền
            order.CurrentState = new PendingState(); // Chuyển sang Chờ duyệt đóng gói
        }

        public void Cancel(Order order)
        {
            order.ShippingStatus = "Cancelled";
            order.CurrentState = new CancelledState();
        }
    }
}