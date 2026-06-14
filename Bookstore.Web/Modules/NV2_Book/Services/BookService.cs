// Vị trí: Bookstore.Web/Modules/NV2_Book/Services/BookService.cs
using System.Collections.Generic;
using System.Linq;
using Bookstore.Core.Interfaces;
using Bookstore.Core.Models.NV2_Book;

namespace Bookstore.Web.Modules.NV2_Book.Services
{
    public class BookService : IBookService
    {
        private readonly IBookRepository _bookRepository;

        // ✨ Tiêm cổng giao tiếp dữ liệu thông qua DI thay vì gọi cứng MockDataStore
        public BookService(IBookRepository bookRepository)
        {
            _bookRepository = bookRepository;
        }

        public List<Book> GetAllBooks() => _bookRepository.GetAll();

        public Book? GetBookById(int id) => _bookRepository.GetById(id);

        public void UpdateStock(int bookId, int quantity)
        {
            var book = _bookRepository.GetById(bookId);
            if (book != null)
            {
                book.StockQuantity = quantity;
                _bookRepository.Update(book);
            }
        }
    }
}