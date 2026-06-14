// Vị trí: Bookstore.Web/Modules/NV5_Payment/Services/OnlinePaymentTemplate.cs
using System;
using Bookstore.Core.Interfaces;
using Bookstore.Core.Models;
using Bookstore.Core.Models.NV4_Order;
using Bookstore.Core.Models.NV5_Payment.Interfaces;

namespace Bookstore.Web.Modules.NV5_Payment.Services
{
    public abstract class OnlinePaymentTemplate : IPaymentStrategy
    {
        protected abstract string PaymentMethodName { get; }

        // ✨ Cải tiến: Nhận thêm IPaymentRepository thông qua tham số hàm xử lý
        public bool ProcessPayment(Order order, IPaymentRepository paymentRepository)
        {
            InitTransaction(order);
            
            if (!ValidateAndSign())
            {
                WriteFailedLog(order.Id);
                return false;
            }

            bool apiResult = SendApiRequest(order.TotalAmount);
            
            // Lưu dữ liệu thông qua Repository sạch
            SavePaymentToRepository(order.Id, order.TotalAmount, apiResult, paymentRepository);

            return apiResult;
        }

        private void InitTransaction(Order order)
        {
            Console.WriteLine($"[Hệ thống] Khởi tạo giao dịch trực tuyến cho đơn hàng ID: {order.Id}");
        }

        private void WriteFailedLog(int orderId)
        {
            Console.WriteLine($"[Lỗi] Xác thực hoặc chữ ký bảo mật thất bại cho đơn hàng ID: {orderId}.");
        }

        private void SavePaymentToRepository(int orderId, decimal amount, bool isSuccess, IPaymentRepository paymentRepository)
        {
            var paymentRecord = new Payment
            {
                Id = paymentRepository.GetAll().Count + 1,
                OrderId = orderId,
                Amount = amount,
                PaymentDate = DateTime.Now,
                PaymentMethod = this.PaymentMethodName,
                IsSuccess = isSuccess
            };
            
            paymentRepository.Add(paymentRecord);
            
            string status = isSuccess ? "Thành công" : "Thất bại";
            Console.WriteLine($"[Repository] Đã lưu lịch sử giao dịch #{paymentRecord.Id} - Trạng thái: {status}");
        }

        protected abstract bool ValidateAndSign();
        protected abstract bool SendApiRequest(decimal amount);
    }
}