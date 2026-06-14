// vị trí: Bookstore.Core/Interfaces/IOrderObserver.cs
namespace Bookstore.Core.Models.NV4_Order.Interfaces
{
    // Interface cho Observer Pattern
    public interface IOrderObserver
    {
        void UpdateOnOrderDelivered(int orderId);
        void UpdateOnOrderCancelled(int orderId);
    }
}