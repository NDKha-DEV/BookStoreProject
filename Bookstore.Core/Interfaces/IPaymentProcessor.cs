// Vị trí: Bookstore.Core/Interfaces/IPaymentProcessor.cs
namespace Bookstore.Core.Interfaces
{
    public interface IPaymentProcessor
    {
        // Thêm tham số int orderId để các Adapter có đủ dữ liệu ghi log đối soát
        bool ProcessPayment(int orderId, decimal amount);
    }
}