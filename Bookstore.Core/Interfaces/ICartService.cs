// vị trí: Bookstore.Core/Interfaces/ICartService.cs
using Bookstore.Core.Models;
namespace Bookstore.Core.Interfaces {
    public interface ICartService {
        void AddToCart(int bookId, int quantity);
        void RemoveFromCart(int bookId);
        decimal GetCartTotal(); // Gọi tính toán cuối cùng sau khi áp Decorator
    }
}