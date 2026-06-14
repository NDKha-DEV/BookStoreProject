// vị trí: Bookstore.Web/Services/InMemoryInventoryStrategy.cs
namespace Bookstore.Web.Services
{
    public class InMemoryInventoryStrategy : IInventoryStrategy
    {
        public int GetStock(int productId, int? variantId = null)
        {
            if (productId <= 0) return 0;
            return 10;
        }

        public decimal GetPrice(int productId, int? variantId = null)
        {
            return productId <= 0 ? 0m : (decimal)(productId * 1.5m + 10);
        }

        public string? GetTitle(int productId)
        {
            if (productId <= 0) return null;
            return $"Product #{productId}";
        }
    }
}
