// vị trí: Bookstore.Core/Models/Order.cs
using System;
using System.Collections.Generic;
using Bookstore.Core.Interfaces;
using Bookstore.Core.Models.NV3_Cart;

namespace Bookstore.Core.Models
{
    public class Order
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public decimal TotalAmount { get; set; }
        public DateTime CreatedDate { get; set; }
        public string PaymentMethod { get; set; } = string.Empty; // "COD" hoặc "ONLINE"
        // Thêm danh sách các mặt hàng được chụp lại từ Giỏ hàng lúc bấm Mua
        public List<CartItem> OrderItems { get; set; } = new List<CartItem>();
        // 🌟 TRỤC TRẠNG THÁI ĐÔI SONG SONG
        public string PaymentStatus { get; set; } = "Unpaid";      // "Unpaid", "Paid", "Refunded"
        public string ShippingStatus { get; set; } = "NotShipped";  // "NotShipped", "Shipping", "Delivered", "Cancelled"

        // Quản lý State Pattern
        public IOrderState CurrentState { get; set; } = null!;

        // Quản lý Observer Pattern
        private readonly List<IOrderObserver> _observers = new List<IOrderObserver>();

        public void RegisterObserver(IOrderObserver observer) => _observers.Add(observer);
        
        public void NotifyObservers()
        {
            foreach (var observer in _observers)
            {
                observer.UpdateOnOrderDelivered(this.Id);
            }
        }

        // Các hàm bọc để State xử lý dịch chuyển linh hoạt
        public void Proceed() => CurrentState.Proceed(this);
        public void Cancel()
        {
            CurrentState.Cancel(this);
            
            // ✨ BỔ SUNG: Phát tín hiệu hủy đơn sang toàn bộ các hệ thống Observer quản lý
            foreach (var observer in _observers)
            {
                observer.UpdateOnOrderCancelled(this.Id);
            }
        }
        public string GetFullStatus() => $"[Giao hàng: {ShippingStatus}] - [Thanh toán: {PaymentStatus}]";
    }
}