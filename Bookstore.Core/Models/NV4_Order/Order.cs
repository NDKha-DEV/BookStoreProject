// vị trí: Bookstore.Core/Models/NV4_Order/Order.cs
using System;
using System.Collections.Generic;
using Bookstore.Core.Models.NV3_Cart;
using Bookstore.Core.Models.NV4_Order.Interfaces;

namespace Bookstore.Core.Models.NV4_Order
{
    public class Order : IOrderSubject
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public decimal TotalAmount { get; set; }
        public DateTime CreatedDate { get; set; }
        public string PaymentMethod { get; set; } = string.Empty; // "COD" hoặc "ONLINE"
        public List<CartItem> OrderItems { get; set; } = new List<CartItem>();
        
        public string PaymentStatus { get; set; } = "Unpaid";      // "Unpaid", "Paid", "Refunded"
        public string ShippingStatus { get; set; } = "NotShipped";  // "NotShipped", "Shipping", "Delivered", "Cancelled"

        public IOrderState CurrentState { get; set; } = null!;

        private readonly List<IOrderObserver> _observers = new List<IOrderObserver>();

        public void RegisterObserver(IOrderObserver observer) => _observers.Add(observer);
        
        public void NotifyObservers()
        {
            foreach (var observer in _observers)
            {
                observer.UpdateOnOrderDelivered(this.Id);
            }
        }

        public void RemoveObserver(IOrderObserver observer) => _observers.Remove(observer);

        public void Proceed() => CurrentState.Proceed(this);
        public void Cancel()
        {
            CurrentState.Cancel(this);
            foreach (var observer in _observers)
            {
                observer.UpdateOnOrderCancelled(this.Id);
            }
        }
        public string GetFullStatus() => $"[Giao hàng: {ShippingStatus}] - [Thanh toán: {PaymentStatus}]";
    }
}