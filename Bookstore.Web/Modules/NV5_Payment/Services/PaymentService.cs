// Vị trí: Bookstore.Web/Modules/NV5_Payment/Services/PaymentService.cs
using System;
using Bookstore.Core.Interfaces;
using Bookstore.Core.Models.NV5_Payment.Interfaces;

namespace Bookstore.Web.Modules.NV5_Payment.Services
{
    public class PaymentService
    {
        private readonly IOrderRepository _orderRepository;
        private readonly IPaymentRepository _paymentRepository;

        // Tiêm lỏng các cổng lưu trữ thông qua hàm khởi tạo công khai
        public PaymentService(IOrderRepository orderRepository, IPaymentRepository paymentRepository)
        {
            _orderRepository = orderRepository;
            _paymentRepository = paymentRepository;
        }

        public bool ProcessOrderPayment(int orderId, string paymentMethod)
        {
            // 1. Tìm đơn hàng thông qua Repo sạch
            var order = _orderRepository.GetById(orderId);
            if (order == null) throw new Exception("Không tìm thấy đơn hàng để thanh toán!");

            var method = paymentMethod.ToUpper();
            order.PaymentMethod = method;

            // 2. STRATEGY PATTERN: Khởi tạo chiến lược
            IPaymentStrategy paymentStrategy = method switch
            {
                "MOMO" => new MoMoPayment(),
                "CARD" => new CardPayment(),
                "VIETQR" => new BankAccountPayment(),
                "COD" => new CODPayment(),
                _ => throw new Exception("Phương thức thanh toán không được hỗ trợ!")
            };

            // 3. Thực thi thanh toán đầu cuối
            bool isSuccess = paymentStrategy.ProcessPayment(order, _paymentRepository);

            // 4. Đồng bộ luồng trạng thái State Pattern
            if (isSuccess)
            {
                if (method == "COD")
                {
                    order.PaymentStatus = "Unpaid";
                }
                else
                {
                    order.PaymentStatus = "Paid";
                }
                
                order.Proceed(); 
                _orderRepository.Update(order); // Đồng bộ thay đổi trạng thái đơn hàng vào Repo
            }
            else
            {
                order.PaymentStatus = "Failed";
                _orderRepository.Update(order);
            }

            return isSuccess;
        }
    }
}