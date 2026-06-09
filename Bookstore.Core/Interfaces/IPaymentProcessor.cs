// 5. NV5: IPaymentProcessor.cs - Cổng Adapter thanh toán chung
namespace Bookstore.Core.Interfaces
{
    public interface IPaymentProcessor
    {
        // Trả về true nếu thanh toán thành công, false nếu thất bại
        bool ProcessPayment(decimal amount);
    }
}