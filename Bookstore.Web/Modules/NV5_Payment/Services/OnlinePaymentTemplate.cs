// vị trí: Bookstore.Web/Modules/NV5_Payment/Services/OnlinePaymentTemplate.cs
using System;
using Bookstore.Core.Interfaces;
using Bookstore.Core.Models;
using Bookstore.Core.Models.NV4_Order;
using Bookstore.Core.Models.NV5_Payment.Interfaces;

namespace Bookstore.Web.Modules.NV5_Payment.Services
{
    public abstract class OnlinePaymentTemplate : IPaymentStrategy
    {
        public bool ProcessPayment(Order order)
        {
            InitTransaction(order);
            
            if (!ValidateAndSign())
            {
                WriteFailedLog(order.Id);
                return false;
            }

            bool apiResult = SendApiRequest(order.TotalAmount);
            
            // 🔥 ĐỒNG BỘ: Lưu dữ liệu giao dịch trực tiếp vào bộ nhớ hệ thống
            SavePaymentToMockData(order.Id, order.TotalAmount, apiResult);

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

        private void SavePaymentToMockData(int orderId, decimal amount, bool isSuccess)
        {
            var paymentRecord = new Payment
            {
                Id = MockDataStore.Payments.Count + 1,
                OrderId = orderId,
                Amount = amount,
                PaymentDate = DateTime.Now,
                PaymentMethod = this.GetType().Name.Replace("Payment", ""),
                IsSuccess = isSuccess
            };
            MockDataStore.Payments.Add(paymentRecord);
            
            string status = isSuccess ? "Thành công" : "Thất bại";
            Console.WriteLine($"[MockDB] Đã lưu lịch sử giao dịch {paymentRecord.Id} - Trạng thái: {status}");
        }

        protected abstract bool ValidateAndSign();
        protected abstract bool SendApiRequest(decimal amount);
    }
}