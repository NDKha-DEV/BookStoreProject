// Vị trí: Bookstore.Core/Models/NV5_Payment/Interfaces/IPaymentStrategy.cs
using Bookstore.Core.Interfaces;
using Bookstore.Core.Models.NV4_Order;

namespace Bookstore.Core.Models.NV5_Payment.Interfaces
{
    public interface IPaymentStrategy
    {
        // Thêm cổng lưu trữ vào chữ ký hàm
        bool ProcessPayment(Order order, IPaymentRepository paymentRepository);
    }
}