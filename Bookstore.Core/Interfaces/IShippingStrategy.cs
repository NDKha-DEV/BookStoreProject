// Vị trí: Bookstore.Core/Interfaces/IShippingStrategy.cs
namespace Bookstore.Core.Interfaces 
{
    public interface IShippingStrategy 
    {
        // Thống nhất dùng tên hàm này cho toàn hệ thống
        decimal CalculateShippingFee(decimal distanceInKm);
    }
}