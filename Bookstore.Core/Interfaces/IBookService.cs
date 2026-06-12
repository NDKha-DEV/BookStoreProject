// Trong file: Bookstore.Core/Interfaces/IBookService.cs
using Bookstore.Core.Models;
using System.Collections.Generic;

namespace Bookstore.Core.Interfaces 
{
    public interface IBookService 
    {
        List<Book> GetAllBooks();
        Book? GetBookById(int id);
        void UpdateStock(int bookId, int quantity); // Dùng để trừ kho khi mua xong
    }
}