// vị trí: Bookstore.Core/Interfaces/IPaymentProcessor.cs
namespace Bookstore.Core.Interfaces
{
    public interface IPaymentProcessor
    {
        // Trả về true nếu thanh toán thành công, false nếu thất bại
        bool ProcessPayment(decimal amount);
    }
}