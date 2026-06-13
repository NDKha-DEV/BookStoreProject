// Vị trí: Bookstore.Web/Modules/NV4_Order/Observers/InventoryObserver.cs
using Bookstore.Core.Interfaces;
using Bookstore.Core.Models;
using System;
using System.Linq;

namespace Bookstore.Web.Modules.NV4_Order.Observers
{
    public class InventoryObserver : IOrderObserver
    {
        public void UpdateOnOrderDelivered(int orderId)
        {
            var order = MockDataStore.Orders.FirstOrDefault(o => o.Id == orderId);
            
            // 🌟 LUỒNG SHOPEE: Chỉ trừ kho thật khi đơn hàng hoàn tất giao thành công và đã nhận tiền
            if (order != null && order.ShippingStatus == "Delivered" && order.PaymentStatus == "Paid")
            {
                Console.WriteLine($"\n[📦 OBSERVER INVENTORY] Kích hoạt cấu trừ kho tự động cho Đơn hàng #{orderId}:");
                
                foreach (var item in order.OrderItems)
                {
                    // Tìm cuốn sách trong kho tĩnh dựa vào BookId lưu trong chi tiết đơn hàng
                    var book = MockDataStore.Books.FirstOrDefault(b => b.Id == item.Product.Id);
                    if (book != null)
                    {
                        // Thực hiện trừ số lượng tồn kho thực tế
                        book.StockQuantity -= item.Quantity;
                        
                        Console.WriteLine($" => Sách ID {book.Id} ('{book.Title}'): Trừ {item.Quantity} cuốn. (Tồn kho còn lại: {book.StockQuantity})");
                    }
                }
            }
        }
        // Thêm hàm này vào bên trong class InventoryObserver:
        public void UpdateOnOrderCancelled(int orderId)
        {
            var order = MockDataStore.Orders.FirstOrDefault(o => o.Id == orderId);
            if (order != null)
            {
                Console.WriteLine($"[ OBSERVER INVENTORY] Đơn hàng #{orderId} đã bị HỦY ở trạng thái {order.CurrentState.GetStatusName()}. Không cần hoàn trả tồn kho vì sản phẩm chưa xuất xưởng.");
            }
        }
    }
}