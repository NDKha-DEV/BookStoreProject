// IShippingStrategy.cs
namespace Bookstore.Core.Interfaces {
    public interface IShippingStrategy {
        decimal CalculateShippingCost(decimal distanceInKm);
    }
}