using Bookstore.Core.Interfaces;
using Bookstore.Core.Models;
using System.Collections.Generic;
using System.Linq;

namespace Bookstore.Web.Modules.NV2_Book.Services
{
    public class BookService : IBookService
    {
        // THÊM TỪ KHÓA static THÀNH: static readonly List<Book>
        private static readonly List<Book> _books = new()
        {
            new PaperBook { Id = 1, Title = "Dac Nhan Tam", Author = "Dale Carnegie", BasePrice = 86000, StockQuantity = 50 },
            new EBook { Id = 2, Title = "Clean Code", Author = "Robert C. Martin", BasePrice = 250000, StockQuantity = 12 },
            new AudioBook { Id = 3, Title = "Sach noi Thao Tung Tam Ly", Author = "Dr. Stephanie", BasePrice = 150000, StockQuantity = 100 }
        };

        public List<Book> GetAllBooks()
        {
            return _books;
        }

        public Book? GetBookById(int id)
        {
            return _books.FirstOrDefault(book => book.Id == id);
        }

        public void UpdateStock(int bookId, int quantity)
        {
            var book = GetBookById(bookId);
            if (book != null)
            {
                book.StockQuantity -= quantity;
            }
        }
    }
}