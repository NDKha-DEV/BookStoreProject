// vị trí: Bookstore.Web/Modules/NV2_Book/Services/BookService.cs
using System.Collections.Generic;
using System.Linq;
using Bookstore.Core.Models;
using Bookstore.Core.Models.NV2_Book;

namespace Bookstore.Web.Modules.NV2_Book.Services
{
    public class BookService : IBookService
    {
        // 🔥 ĐỒNG BỘ: Đã xóa sổ hoàn toàn _books static riêng tư, chuyển sang dùng MockDataStore chung
        public List<Book> GetAllBooks() => MockDataStore.Books;

        public Book? GetBookById(int id)
        {
            return MockDataStore.Books.FirstOrDefault(b => b.Id == id);
        }

        public void UpdateStock(int bookId, int quantity)
        {
            var book = MockDataStore.Books.FirstOrDefault(b => b.Id == bookId);
            if (book != null)
            {
                book.StockQuantity = quantity;
            }
        }
    }
}