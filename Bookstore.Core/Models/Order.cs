// Trong file: Bookstore.Core/Models/Order.cs
using Bookstore.Core.Interfaces;

namespace Bookstore.Core.Models
{
    public class Order : IOrderSubject
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public List<CartItem> OrderItems { get; set; } = new List<CartItem>();
        public decimal TotalAmount { get; set; }
        public DateTime CreatedDate { get; set; } = DateTime.Now;

        // Lưu trữ State hiện tại của Đơn hàng (State Pattern)
        // Bạn dặn thành viên NV4 khởi tạo trạng thái mặc định là PendingState (Chờ duyệt)
        public IOrderState CurrentState { get; set; } = null!; 

        // Lưu danh sách các bên đăng ký nhận thông báo (Observer Pattern)
        private readonly List<IOrderObserver> _observers = new List<IOrderObserver>();

        // Thực thi các hàm của IOrderSubject (Observer)
        public void RegisterObserver(IOrderObserver observer) => _observers.Add(observer);
        public void RemoveObserver(IOrderObserver observer) => _observers.Remove(observer);
        
        public void NotifyObservers()
        {
            foreach (var observer in _observers)
            {
                observer.UpdateOnOrderDelivered(this.Id);
            }
        }

        // Hàm trigger chuyển trạng thái (Do Admin bấm)
        public void Proceed() => CurrentState.Proceed(this);
        public void Cancel() => CurrentState.Cancel(this);
    }
}