// Vị trí: Bookstore.Web/Modules/NV2_Book/BookFactory.cs
using Bookstore.Core.Models;

namespace Bookstore.Web.Modules.NV2_Book
{
    public abstract class BookCreator
    {
        public abstract Book CreateBook(int id, string title, string author, decimal basePrice, int stock);
    }

    public class PhysicalBookCreator : BookCreator
    {
        public override Book CreateBook(int id, string title, string author, decimal basePrice, int stock)
        {
            // Tự động gán thuộc tính đặc thù của Sách giấy
            return new PhysicalBook { Id = id, Title = title, Author = author, BasePrice = basePrice, StockQuantity = stock, WeightInGram = 500 };
        }
    }

    public class EBookCreator : BookCreator
    {
        public override Book CreateBook(int id, string title, string author, decimal basePrice, int stock)
        {
            // Tự động gán thuộc tính đặc thù của EBook
            return new EBook { Id = id, Title = title, Author = author, BasePrice = basePrice, StockQuantity = stock, FileSizeInMb = 10.5 };
        }
    }
}