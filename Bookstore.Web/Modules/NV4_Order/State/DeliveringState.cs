// Trong file: Bookstore.Web/Modules/NV4_Order/States/DeliveringState.cs
using Bookstore.Core.Interfaces;
using Bookstore.Core.Models;

namespace Bookstore.Web.Modules.NV4_Order.States
{
    public class DeliveringState : IOrderState
    {
        public string GetStatusName() => "Đang giao hàng";

        public void Proceed(Order order)
        {
            // Từ Đang giao chuyển sang Đã giao
            order.CurrentState = new DeliveredState();
            
            // ĐẶC BIỆT: Khi đơn hàng chuyển sang Đã giao thành công, kích hoạt Observer!
            order.NotifyObservers();
        }

        public void Cancel(Order order)
        {
            // Đang đi giao thì không cho phép khách bấm Hủy bừa bãi nữa
            throw new InvalidOperationException("Đơn hàng đang trên đường giao, không thể hủy!");
        }
    }
}