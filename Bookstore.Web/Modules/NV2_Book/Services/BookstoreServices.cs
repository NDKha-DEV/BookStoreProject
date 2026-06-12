using Bookstore.Core.Models;
using System.Collections.Generic;
using System.Linq;

namespace Bookstore.Core.Interfaces
{//Strategy Pattern 
        public interface ISearchStrategy
    {
        List<Book> Search(List<Book> books, string keyword);
    }
}

namespace Bookstore.Web.Modules.NV2_Book.Services
{
    using Bookstore.Core.Models;
    using Bookstore.Core.Interfaces;

    // xử lý logic nghiệp vụ liên quan đến sách, bao gồm quản lý tồn kho và thông tin sách
        public class BookService : IBookService
    {
        // Danh sách dữ liệu mẫu (Mock Data) hoạt động trong bộ nhớ RAM
        private static readonly List<Book> _books = new List<Book>
        {
            new PaperBook { Id = 1, Title = "Dac Nhan Tam", Author = "Dale Carnegie", BasePrice = 86000, StockQuantity = 50 },
            new EBook { Id = 2, Title = "Clean Code", Author = "Robert C. Martin", BasePrice = 250000, StockQuantity = 12 },
            new AudioBook { Id = 3, Title = "Sach noi Thao Tung Tam Ly", Author = "Dr. Stephanie", BasePrice = 150000, StockQuantity = 100 }
        };

        // Hàm lấy toàn bộ danh sách sách hiện có
        public List<Book> GetAllBooks() => _books;

        // Hàm lấy chi tiết một cuốn sách theo mã định danh ID
        public Book? GetBookById(int id)
        {
            return _books.FirstOrDefault(b => b.Id == id);
        }

        // Hàm cập nhật số lượng tồn kho của sách khi nhập/xuất hàng
        public void UpdateStock(int bookId, int quantity)
        {
            var book = _books.FirstOrDefault(b => b.Id == bookId);
            if (book != null)
            {
                book.StockQuantity = quantity;
            }
        }
    }

    // xử lý logic nghiệp vụ liên quan đến danh mục sản phẩm, bao gồm quản lý thông tin danh mục và kiểm tra trùng lặp tên danh mục
    // quản lý thêm,cập nhật, xóa 
        public class CategoryService
    {
        // Danh sách dữ liệu mẫu của Danh mục sản phẩm
        private static readonly List<Category> _categories = new List<Category>
        {
            new Category { Id = 1, Name = "Sách Văn Học", Description = "Tiểu thuyết, truyện ngắn, thơ" },
            new Category { Id = 2, Name = "Sách Công Nghệ", Description = "Lập trình, cơ sở dữ liệu, AI" }
        };

        // Xem toàn bộ danh sách danh mục
        public List<Category> GetAllCategories() => _categories;

        // Lấy chi tiết một danh mục theo ID
        public Category? GetCategoryById(int id) => _categories.FirstOrDefault(c => c.Id == id);

        // Kiểm tra xem tên danh mục có bị trùng lặp trong hệ thống hay không (Phục vụ ngoại lệ UC 4.38)
        public bool IsNameExists(string name, int? excludeId = null)
        {
            if (string.IsNullOrWhiteSpace(name)) return false;
            return _categories.Any(c => c.Name.ToLower() == name.Trim().ToLower() && c.Id != excludeId);
        }

        // Thêm mới một danh mục (Tự động tăng ID)
        public void AddCategory(Category c)
        {
            c.Id = _categories.Count > 0 ? _categories.Max(x => x.Id) + 1 : 1;
            _categories.Add(c);
        }

        // Cập nhật thông tin danh mục hiện tại
        public void UpdateCategory(Category c)
        {
            var existingCategory = GetCategoryById(c.Id);
            if (existingCategory != null)
            {
                existingCategory.Name = c.Name;
                existingCategory.Description = c.Description;
            }
        }

        // Xóa một danh mục khỏi danh sách static
        public void DeleteCategory(int id)
        {
            var category = GetCategoryById(id);
            if (category != null)
            {
                _categories.Remove(category);
            }
        }
    }

    //tìm kiếm theo strategy pattern để linh hoạt thay đổi thuật toán tìm kiếm mà không cần sửa đổi code của SearchService, chỉ cần tạo thêm các lớp chiến lược mới nếu muốn mở rộng cách tìm kiếm.
    // 1: Tìm kiếm sách theo Tên sách
    public class SearchByTitleStrategy : ISearchStrategy
    {
        public List<Book> Search(List<Book> books, string keyword)
        {
            if (string.IsNullOrEmpty(keyword)) return books;
            return books.Where(b => b.Title.ToLower().Contains(keyword.ToLower())).ToList();
        }
    }

    //  2: Tìm kiếm sách theo tên Tác giả
    public class SearchByAuthorStrategy : ISearchStrategy
    {
        public List<Book> Search(List<Book> books, string keyword)
        {
            if (string.IsNullOrEmpty(keyword)) return books;
            return books.Where(b => b.Author.ToLower().Contains(keyword.ToLower())).ToList();
        }
    }

    // 3: Tìm kiếm sách theo định dạng loại sách (Paper, EBook, Audio)
    public class SearchByTypeStrategy : ISearchStrategy
    {
        public List<Book> Search(List<Book> books, string keyword)
        {
            if (string.IsNullOrEmpty(keyword)) return books;
            return books.Where(b => b.BookType.ToLower().Contains(keyword.ToLower())).ToList();
        }
    }

    // Khối Context đóng vai trò điều khiển và vận hành chiến lược tìm kiếm linh hoạt
    public class SearchService
    {
        private readonly ISearchStrategy _strategy;

        // Tiêm thuật toán (Strategy) cần dùng thông qua hàm khởi tạo
        public SearchService(ISearchStrategy strategy)
        {
            _strategy = strategy;
        }

        // Thực thi thuật toán được chỉ định
        public List<Book> Search(List<Book> books, string keyword)
        {
            return _strategy.Search(books, keyword);
        }
    }
}