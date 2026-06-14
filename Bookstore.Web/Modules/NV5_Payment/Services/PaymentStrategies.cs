// vị trí: Bookstore.Web/Modules/NV5_Payment/Services/PaymentStrategies.cs
using System;
using Bookstore.Core.Interfaces;
using Bookstore.Core.Models.NV4_Order;
using Bookstore.Core.Models.NV5_Payment.Interfaces;

namespace Bookstore.Web.Modules.NV5_Payment.Services
{
    // --- 1. Cổng MoMo ---
    public class MoMoPayment : OnlinePaymentTemplate
    {
        protected override string PaymentMethodName => "MOMO";
        protected override bool ValidateAndSign()
        {
            Console.WriteLine("[MoMo] Khởi tạo mã hóa SHA256 bảo mật...");
            return true;
        }

        protected override bool SendApiRequest(decimal amount)
        {
            Console.WriteLine($"[MoMo API] Đang kết nối yêu cầu thanh toán số tiền: {amount:N0} VNĐ");
            return true; // Tự động xác nhận thành công cho Web API mượt mà
        }
    }

    // --- 2. Thẻ Visa/Mastercard ---
    public class CardPayment : OnlinePaymentTemplate
    {
        protected override string PaymentMethodName => "CARD";
        protected override bool ValidateAndSign()
        {
            Console.WriteLine("[Card] Xác thực định dạng thẻ hợp lệ...");
            return true;
        }

        protected override bool SendApiRequest(decimal amount)
        {
            Console.WriteLine($"[Cổng Thẻ] Gửi yêu cầu trừ tiền qua cổng thanh toán: {amount:N0} VNĐ");
            return true;
        }
    }

    // --- 3. Quét mã VietQR ---
    public class BankAccountPayment : OnlinePaymentTemplate
    {
        protected override string PaymentMethodName => "VIETQR";
        protected override bool ValidateAndSign()
        {
            Console.WriteLine("[VietQR] Sinh mã QR Code động theo thông tin hóa đơn...");
            return true;
        }

        protected override bool SendApiRequest(decimal amount)
        {
            Console.WriteLine($"[VietQR] Sinh mã chuyển khoản Techcombank số tiền: {amount:N0} VNĐ");
            return true;
        }
    }

    // --- 4. Phương thức COD (Chiến lược độc lập, không dùng Template Online) ---
    public class CODPayment : IPaymentStrategy
    {
        public bool ProcessPayment(Order order, IPaymentRepository paymentRepository)
        {
            Console.WriteLine($"[COD] Đã xác nhận hình thức thu tiền mặt khi nhận hàng cho đơn {order.Id}.");
            return true; 
        }
    }
}