// Vị trí: Bookstore.Web/Modules/NV4_Order/Services/ShippingStrategies.cs
using Bookstore.Core.Models.NV4_Order.Interfaces;

namespace Bookstore.Web.Modules.NV4_Order.Services
{
    public class StandardShippingStrategy : IShippingStrategy
    {
        public decimal CalculateShippingFee(decimal distanceInKm) => distanceInKm * 5000m; // 5k/1km
    }

    public class ExpressShippingStrategy : IShippingStrategy
    {
        public decimal CalculateShippingFee(decimal distanceInKm) => 30000m + (distanceInKm * 2000m); // Cố định 30k + biên độ
    }
}