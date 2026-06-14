// Vị trí: Bookstore.Web/Modules/NV5_Payment/Services/PaymentService.cs
using System;
using System.Linq;
using Bookstore.Core.Models;
using Bookstore.Core.Models.NV5_Payment.Interfaces;
using Bookstore.Web.Modules.NV4_Order.States;

namespace Bookstore.Web.Modules.NV5_Payment.Services
{
    public class PaymentService
    {
        public bool ProcessOrderPayment(int orderId, string paymentMethod)
        {
            // 1. Tìm đơn hàng cần thanh toán trong hệ thống
            var order = MockDataStore.Orders.FirstOrDefault(o => o.Id == orderId);
            if (order == null) throw new Exception("Không tìm thấy đơn hàng để thanh toán!");

            var method = paymentMethod.ToUpper();
            order.PaymentMethod = method; // Cập nhật phương thức người dùng chọn vào đơn hàng

            // 2. STRATEGY PATTERN: NV5 quyết định chiến lược xử lý tiền
            IPaymentStrategy paymentStrategy = method switch
            {
                "MOMO" => new MoMoPayment(),
                "CARD" => new CardPayment(),
                "VIETQR" => new BankAccountPayment(),
                "COD" => new CODPayment(),
                _ => throw new Exception("Phương thức thanh toán không được hỗ trợ!")
            };

            // 3. Thực thi thanh toán
            bool isSuccess = paymentStrategy.ProcessPayment(order);

            // 4. Đồng bộ luồng trạng thái với NV4 sau khi có kết quả thanh toán
            if (isSuccess)
            {
                if (method == "COD")
                {
                    order.PaymentStatus = "Unpaid"; // COD thì nhận hàng mới trả tiền
                }
                else
                {
                    order.PaymentStatus = "Paid"; // Online thành công thì đánh dấu Đã trả
                }
                
                // ĐA HÌNH STATE PATTERN: Kích hoạt chuyển trạng thái từ Chờ thanh toán -> Chờ duyệt giao hàng (PendingState)
                order.Proceed(); 
            }
            else
            {
                order.PaymentStatus = "Failed";
                // Giữ nguyên trạng thái AwaitingPaymentState để khách có thể thử lại
            }

            return isSuccess;
        }
    }
}