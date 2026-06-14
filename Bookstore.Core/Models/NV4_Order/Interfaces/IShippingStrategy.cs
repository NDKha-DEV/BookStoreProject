// Vị trí: Bookstore.Core/Models/NV4_Order/Interfaces/IShippingStrategy.cs
namespace Bookstore.Core.Models.NV4_Order.Interfaces 
{
    public interface IShippingStrategy 
    {
        // Thống nhất dùng tên hàm này cho toàn hệ thống
        decimal CalculateShippingFee(decimal distanceInKm);
    }
}