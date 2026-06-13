// Vị trí: Bookstore.Web/Controllers/BookController.cs
using Microsoft.AspNetCore.Mvc;
using Bookstore.Core.Interfaces;
using Bookstore.Core.Models;
using Bookstore.Web.Modules.NV2_Book;
using Bookstore.Web.Modules.NV2_Book.Strategies;

namespace Bookstore.Web.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class BookController : ControllerBase
    {
        private readonly IBookService _bookService;
        // Ép kiểu ngược (Cast) để dùng riêng cho các hàm Strategy/Category không có trong Interface gốc
        private readonly BookService _concreteBookService; 

        public BookController(IBookService bookService)
        {
            _bookService = bookService;
            _concreteBookService = (BookService)bookService;
        }

        [HttpGet("search")]
        public IActionResult Search([FromQuery] string keyword, [FromQuery] string strategyType = "title")
        {
            if (strategyType.ToLower() == "author")
            {
                _concreteBookService.SetSearchStrategy(new SearchByAuthorStrategy());
            }
            else
            {
                _concreteBookService.SetSearchStrategy(new SearchByTitleStrategy());
            }

            var results = _concreteBookService.SearchBooks(keyword);
            return Ok(new {
                StrategyUsed = strategyType.ToUpper() == "AUTHOR" ? "Tìm theo Tác giả" : "Tìm theo Tiêu đề",
                TotalCount = results.Count,
                Data = results
            });
        }

        [HttpGet("{id}")]
        public IActionResult GetById(int id)
        {
            try
            {
                var book = _bookService.GetBookById(id);
                return Ok(book);
            }
            catch (Exception ex)
            {
                return NotFound(new { Message = ex.Message });
            }
        }

        [HttpPost("add-book")]
        public IActionResult AddBook([FromQuery] string type, [FromQuery] string title, [FromQuery] string author, [FromQuery] decimal price, [FromQuery] int stock, [FromQuery] int categoryId)
        {
            _concreteBookService.AddBook(type, title, author, price, stock, categoryId);
            return Ok(new { Message = "Thêm sách mới qua Factory Method thành công!" });
        }

        // ==========================================
        // 📚 QUẢN LÝ DANH MỤC SẢN PHẨM (CRUD CATEGORY)
        // ==========================================

        [HttpGet("categories")]
        public IActionResult GetCategories() => Ok(_concreteBookService.GetAllCategories());

        [HttpPost("categories")]
        public IActionResult CreateCategory([FromQuery] string name)
        {
            _concreteBookService.CreateCategory(name);
            return Ok(new { Message = "Tạo danh mục mới thành công!" });
        }

        [HttpDelete("categories/{id}")]
        public IActionResult DeleteCategory(int id)
        {
            _concreteBookService.DeleteCategory(id);
            return Ok(new { Message = "Xóa danh mục thành công!" });
        }
    }
}