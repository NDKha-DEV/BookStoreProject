// vị trí: Bookstore.Core/Models/NV2_Book/IBookService.cs
using System.Collections.Generic;

namespace Bookstore.Core.Models.NV2_Book
{
    public interface IBookService 
    {
        List<Book> GetAllBooks();
        Book? GetBookById(int id);
        void UpdateStock(int bookId, int quantity); 
    }
}