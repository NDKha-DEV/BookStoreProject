// Vị trí: Bookstore.Core/Models/NV4_Order/Interfaces/IOrderSubject.cs
namespace Bookstore.Core.Models.NV4_Order.Interfaces {
    public interface IOrderSubject {
        void RegisterObserver(IOrderObserver observer);
        void RemoveObserver(IOrderObserver observer);
        void NotifyObservers();
    }
}