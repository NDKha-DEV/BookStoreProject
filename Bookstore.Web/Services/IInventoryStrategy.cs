namespace Bookstore.Web.Services
{
    public interface IInventoryStrategy
    {
        int GetStock(int productId, int? variantId = null);
        decimal GetPrice(int productId, int? variantId = null);
        string? GetTitle(int productId);
    }
}
