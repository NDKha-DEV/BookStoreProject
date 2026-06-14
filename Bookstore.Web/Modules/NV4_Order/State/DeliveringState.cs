// Vị trí: Bookstore.Web/Modules/NV4_Order/States/DeliveringState.cs
using System;
using Bookstore.Core.Models.NV4_Order;
using Bookstore.Core.Models.NV4_Order.Interfaces;

namespace Bookstore.Web.Modules.NV4_Order.States
{
    public class DeliveringState : IOrderState
    {
        public string GetStatusName() => "Đang trong quá trình giao hàng";

        public void Proceed(Order order)
        {
            order.ShippingStatus = "Delivered"; // Xác nhận giao hàng thành công

            // 🌟 ĐỒNG BỘ LUỒNG COD: Nhận hàng thành công đồng nghĩa với việc đã thu được tiền mặt
            if (order.PaymentMethod == "COD")
            {
                order.PaymentStatus = "Paid";
            }

            // Kích hoạt hệ thống Observer tự động cập nhật hệ sinh thái
            order.NotifyObservers();
            
            order.CurrentState = new DeliveredState();
        }

        public void Cancel(Order order)
        {
            throw new InvalidOperationException("Hàng đã giao cho Shipper đi phát, không thể hủy ngang giữa chừng!");
        }
    }
}