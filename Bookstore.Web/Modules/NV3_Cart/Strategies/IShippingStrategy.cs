// Vị trí: Bookstore.Web/Modules/NV3_Cart/Strategies/IShippingStrategy.cs
using Bookstore.Core.Interfaces; // ✨ ĐÃ THÊM: Để kế thừa Interface từ Core

namespace Bookstore.Web.Modules.NV3_Cart.Strategies
{
    // Giao hàng nội thành / Gần cửa hàng (< 5km)
    public class StandardShippingStrategy : IShippingStrategy // Kế thừa từ Core
    {
        public decimal CalculateShippingFee(decimal distanceInKm) => 15000; 
    }

    // Giao hàng nhanh ngoại thành / Xa (> 5km)
    public class ExpressShippingStrategy : IShippingStrategy // Kế thừa từ Core
    {
        public decimal CalculateShippingFee(decimal distanceInKm) => 15000 + (distanceInKm * 2000); 
    }
}