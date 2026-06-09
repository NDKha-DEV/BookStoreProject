// IOrderSubject.cs (Để class Order kế thừa nhằm quản lý danh sách Observer)
namespace Bookstore.Core.Interfaces {
    public interface IOrderSubject {
        void RegisterObserver(IOrderObserver observer);
        void RemoveObserver(IOrderObserver observer);
        void NotifyObservers();
    }
}