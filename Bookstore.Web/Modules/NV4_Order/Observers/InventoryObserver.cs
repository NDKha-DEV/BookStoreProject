// Vị trí: Bookstore.Web/Modules/NV4_Order/Observers/InventoryObserver.cs
using System;
using System.Linq;
using Bookstore.Core.Models;
using Bookstore.Core.Models.NV4_Order.Interfaces;

namespace Bookstore.Web.Modules.NV4_Order.Observers
{
    public class InventoryObserver : IOrderObserver
    {
        public void UpdateOnOrderDelivered(int orderId)
        {
            var order = MockDataStore.Orders.FirstOrDefault(o => o.Id == orderId);
            if (order != null && order.ShippingStatus == "Delivered" && order.PaymentStatus == "Paid")
            {
                Console.WriteLine($"\n[ OBSERVER INVENTORY] Kích hoạt trừ kho Đơn hàng #{orderId}:");
                foreach (var item in order.OrderItems)
                {
                    // Đấu nối chuẩn xác sang kho sách thực tế của NV2
                    var book = MockDataStore.Books.FirstOrDefault(b => b.Id == item.Product.Id);
                    if (book != null)
                    {
                        book.StockQuantity -= item.Quantity;
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