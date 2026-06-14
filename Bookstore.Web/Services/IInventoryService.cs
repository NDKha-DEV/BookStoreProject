// vị trí: Bookstore.Web/Services/IInventoryService.cs
namespace Bookstore.Web.Services
{
    public interface IInventoryService
    {
        int GetStock(int productId, int? variantId = null);
        decimal GetPrice(int productId, int? variantId = null);
        string? GetTitle(int productId);
    }
}
