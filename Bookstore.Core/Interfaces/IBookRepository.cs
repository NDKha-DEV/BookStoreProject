// Vị trí: Bookstore.Core/Interfaces/IBookRepository.cs
using System.Collections.Generic;
using Bookstore.Core.Models.NV2_Book;

namespace Bookstore.Core.Interfaces {
    public interface IBookRepository {
        Book? GetById(int id);
        List<Book> GetAll();
        void Add(Book book);      // ✨ Bổ sung mới
        void Update(Book book);   // ✨ Bổ sung mới
        void Delete(int id);      // ✨ Bổ sung mới
    }
}