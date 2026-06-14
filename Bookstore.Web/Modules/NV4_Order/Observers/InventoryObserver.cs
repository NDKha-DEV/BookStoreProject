// Vị trí: Bookstore.Web/Modules/NV4_Order/Observers/InventoryObserver.cs
using System;
using System.Linq;
using Bookstore.Core.Models;
using Bookstore.Core.Interfaces;
using Bookstore.Core.Models.NV4_Order.Interfaces;

namespace Bookstore.Web.Modules.NV4_Order.Observers
{
    public class InventoryObserver : IOrderObserver
    {
        private readonly IOrderRepository _orderRepository;
        private readonly IBookRepository _bookRepository;

        public InventoryObserver(IOrderRepository orderRepository, IBookRepository bookRepository)
        {
            _orderRepository = orderRepository;
            _bookRepository = bookRepository;
        }
        public void UpdateOnOrderDelivered(int orderId)
        {
            var order = _orderRepository.GetById(orderId);
            if (order != null && order.ShippingStatus == "Delivered" && order.PaymentStatus == "Paid")
            {
                Console.WriteLine($"\n[📦 OBSERVER INVENTORY] Kích hoạt trừ kho Đơn hàng #{orderId}:");
                foreach (var item in order.OrderItems)
                {
                    var book = _bookRepository.GetById(item.Product.Id);
                    if (book != null)
                    {
                        book.StockQuantity -= item.Quantity;
                        _bookRepository.Update(book); // Đồng bộ trạng thái kho qua Repo
                        Console.WriteLine($" => Sách '{book.Title}': Trừ {item.Quantity} cuốn. Còn lại: {book.StockQuantity}");
                    }
                }
            }
        }
        public void UpdateOnOrderCancelled(int orderId)
        {
            Console.WriteLine($"[OBSERVER INVENTORY] Đơn hàng #{orderId} đã bị HỦY. Không trừ kho.");
        }
    }
}