// IOrderObserver.cs (Dành cho Observer Pattern)
namespace Bookstore.Core.Interfaces
{
    // Interface cho Observer Pattern
    public interface IOrderObserver
    {
        void UpdateOnOrderDelivered(int orderId);
    }
}