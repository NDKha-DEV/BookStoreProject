// vị trí: Bookstore.Web/Services/InventoryService.cs
using System.Collections.Concurrent;

namespace Bookstore.Web.Services
{
    // Simple in-memory inventory stub. Modify later to call real product service.
    public class InventoryService : IInventoryService
    {
        private readonly ConcurrentDictionary<int, int> _stock = new();

        public InventoryService()
        {
            // default seed: every product has stock 10
            for (int i = 1; i <= 100; i++) _stock[i] = 10;
        }

        public int GetStock(int productId, int? variantId = null)
        {
            if (_stock.TryGetValue(productId, out var s)) return s;
            return 0;
        }

        public decimal GetPrice(int productId, int? variantId = null)
        {
            // stub: price = productId * 1.5 + 10
            return (decimal)(productId * 1.5m + 10);
        }

        public string? GetTitle(int productId)
        {
            return $"Product #{productId}";
        }
    }
}
