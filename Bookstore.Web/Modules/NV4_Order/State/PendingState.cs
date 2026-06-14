// Vị trí: Bookstore.Web/Modules/NV4_Order/States/PendingState.cs
using System;
using Bookstore.Core.Models.NV4_Order;
using Bookstore.Core.Models.NV4_Order.Interfaces;

namespace Bookstore.Web.Modules.NV4_Order.States
{
    public class PendingState : IOrderState
    {
        public string GetStatusName() => "Chờ duyệt và đóng gói";

        public void Proceed(Order order)
        {
            // Kiểm tra ràng buộc: Nếu chọn thanh toán Online mà chưa trả tiền thì cấm duyệt
            if (order.PaymentMethod != "COD" && order.PaymentStatus == "Unpaid")
            {
                throw new InvalidOperationException("Đơn hàng trực tuyến chưa được thanh toán thành công. Không thể duyệt giao hàng!");
            }

            // Hợp lệ thì chuyển sang Đang giao
            order.ShippingStatus = "Shipping";
            order.CurrentState = new DeliveringState();
        }

        public void Cancel(Order order)
        {
            order.ShippingStatus = "Cancelled";
            // Nếu khách đã trả tiền trực tuyến mà hủy đơn, chuyển trạng thái tiền sang Chờ hoàn tiền
            if (order.PaymentStatus == "Paid")
            {
                order.PaymentStatus = "Refunded";
            }
            order.CurrentState = new CancelledState();
        }
    }
}