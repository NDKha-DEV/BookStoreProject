// Vị trí: Bookstore.Core/Models/NV5_Payment/Interfaces/IPaymentStrategy.cs
using Bookstore.Core.Models.NV4_Order;

namespace Bookstore.Core.Models.NV5_Payment.Interfaces
{
    public interface IPaymentStrategy
    {
        bool ProcessPayment(Order order);
    }
}