// Vị trí: Bookstore.Infrastructure/Repositories/MockBookRepository.cs
using System.Collections.Generic;
using System.Linq;
using Bookstore.Core.Interfaces;
using Bookstore.Core.Models;
using Bookstore.Core.Models.NV2_Book;

namespace Bookstore.Infrastructure.Repositories {
    public class MockBookRepository : IBookRepository {
        public Book? GetById(int id) => MockDataStore.Books.FirstOrDefault(b => b.Id == id);
        public List<Book> GetAll() => MockDataStore.Books;
        
        public void Add(Book book) => MockDataStore.Books.Add(book);
        public void Update(Book book) { /* RAM tự động lưu theo tham chiếu Object */ }
        
        public void Delete(int id) {
            var book = GetById(id);
            if (book != null) MockDataStore.Books.Remove(book);
        }
    }
}