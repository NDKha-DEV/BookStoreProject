// vị trí: Bookstore.Core/Interfaces/IOrderObserver.cs
namespace Bookstore.Core.Interfaces
{
    // Interface cho Observer Pattern
    public interface IOrderObserver
    {
        void UpdateOnOrderDelivered(int orderId);
        void UpdateOnOrderCancelled(int orderId);
    }
}