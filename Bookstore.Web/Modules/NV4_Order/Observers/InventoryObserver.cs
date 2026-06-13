// Trong file: Bookstore.Web/Modules/NV4_Order/Observers/InventoryObserver.cs
using Bookstore.Core.Interfaces;
using Bookstore.Core.Models;

namespace Bookstore.Web.Modules.NV4_Order.Observers
{
    public class InventoryObserver : IOrderObserver
    {
        public void UpdateOnOrderDelivered(int orderId)
        {
            var order = MockDataStore.Orders.FirstOrDefault(o => o.Id == orderId);
            if (order != null)
            {
                foreach (var item in order.OrderItems)
                {
                    // Kích hoạt trừ kho
                    item.Product.StockQuantity -= item.Quantity;
                    Console.WriteLine($"[OBSERVER LOG] Đã tự động trừ {item.Quantity} cuốn sản phẩm ID:{item.Product.Id} trong kho.");
                }
            }
        }
    }
}