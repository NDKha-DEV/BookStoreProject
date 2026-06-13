// Vị trí: Bookstore.Web/Modules/NV2_Book/BookService.cs
using Bookstore.Core.Interfaces; // ✨ ĐÃ THÊM: Để kế thừa Interface
using Bookstore.Core.Models;
using Bookstore.Web.Modules.NV2_Book.Strategies;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Bookstore.Web.Modules.NV2_Book
{
    // ✨ ĐÃ SỬA: Kế thừa trực tiếp từ IBookService của tầng Core
    public class BookService : IBookService
    {
        private ISearchStrategy _searchStrategy = new SearchByTitleStrategy();

        public void SetSearchStrategy(ISearchStrategy strategy) => _searchStrategy = strategy;

        public List<Book> SearchBooks(string keyword) => _searchStrategy.Filter(MockDataStore.Books, keyword);

        // ✨ ĐÃ BỔ SUNG: Thực thi hàm lấy toàn bộ sách của Interface
        public List<Book> GetAllBooks()
        {
            return MockDataStore.Books;
        }

        public Book GetBookById(int id)
        {
            return MockDataStore.Books.FirstOrDefault(b => b.Id == id) 
                   ?? throw new Exception("Không tìm thấy sản phẩm yêu cầu!");
        }

        // ✨ ĐÃ BỔ SUNG: Thực thi hàm cập nhật kho (để kết nối luồng Observer của NV4)
        public void UpdateStock(int bookId, int quantity)
        {
            var book = MockDataStore.Books.FirstOrDefault(b => b.Id == bookId);
            if (book != null)
            {
                book.StockQuantity -= quantity; // Trừ kho khi có đơn hàng thành công
                Console.WriteLine($"[ OBSERVER LOG] Cập nhật kho sách ID #{bookId}: Còn lại {book.StockQuantity} cuốn.");
            }
        }

        public void AddBook(string type, string title, string author, decimal price, int stock, int categoryId)
        {
            BookCreator creator = type.ToUpper() == "EBOOK" ? new EBookCreator() : new PhysicalBookCreator();
            int nextId = MockDataStore.Books.Count + 1;
            var newBook = creator.CreateBook(nextId, title, author, price, stock);
            newBook.CategoryId = categoryId;
            MockDataStore.Books.Add(newBook);
        }

        public void UpdateBook(int id, string title, decimal price, int stock)
        {
            var book = GetBookById(id);
            book.Title = title;
            book.BasePrice = price;
            book.StockQuantity = stock;
        }

        // ==========================================
        // CRUD DANH MỤC (CATEGORY)
        // ==========================================
        public List<Category> GetAllCategories() => MockDataStore.Categories;

        public void CreateCategory(string name)
        {
            MockDataStore.Categories.Add(new Category { Id = MockDataStore.Categories.Count + 1, Name = name });
        }

        public void UpdateCategory(int id, string newName)
        {
            var cate = MockDataStore.Categories.FirstOrDefault(c => c.Id == id);
            if (cate != null) cate.Name = newName;
        }

        public void DeleteCategory(int id)
        {
            var cate = MockDataStore.Categories.FirstOrDefault(c => c.Id == id);
            if (cate != null) MockDataStore.Categories.Remove(cate);
        }
    }
}