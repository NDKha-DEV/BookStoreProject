// vị trí: Bookstore.Core/Interfaces/IOrderSubject.cs
namespace Bookstore.Core.Interfaces {
    public interface IOrderSubject {
        void RegisterObserver(IOrderObserver observer);
        void RemoveObserver(IOrderObserver observer);
        void NotifyObservers();
    }
}