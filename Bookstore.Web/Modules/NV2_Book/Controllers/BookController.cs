using Microsoft.AspNetCore.Mvc;
using System.Linq;
using Bookstore.Core.Models.NV2_Book;
using Bookstore.Web.Modules.NV2_Book.Services;

namespace Bookstore.Web.Modules.NV2_Book.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class BookController : ControllerBase
    {
        private readonly IBookService _bookService;
        public BookController(IBookService bookService) 
        {
            _bookService = bookService;
        }

        [HttpGet]
        public IActionResult GetAll() => Ok(_bookService.GetAllBooks());

        [HttpGet("{id}")]
        public IActionResult GetById(int id)
        {
            var book = _bookService.GetAllBooks().FirstOrDefault(b => b.Id == id);
            if (book == null) 
                return NotFound(new { message = $"Không tìm thấy sách có ID = {id}" });
            
            return Ok(book);
        }

        [HttpGet("search")]
        public IActionResult Search([FromQuery] string type, [FromQuery] string keyword)
        {
            var allBooks = _bookService.GetAllBooks();
            ISearchStrategy strategy = type?.ToLower() switch
            {
                "author" => new SearchByAuthorStrategy(),
                "type" => new SearchByTypeStrategy(),
                _ => new SearchByTitleStrategy()
            };
            var result = new SearchService(strategy).Search(allBooks, keyword);
            return (result == null || result.Count == 0) ? NotFound(new { message = "Không tồn tại sản phẩm" }) : Ok(result);
        }

        [HttpPost]
        public IActionResult Create([FromQuery] string bookType, [FromQuery] int categoryId, [FromQuery] string title, [FromQuery] string author, [FromQuery] decimal price, [FromQuery] int stock, [FromQuery] string imageUrl = "")
        {
            if (string.IsNullOrEmpty(title) || price <= 0) return BadRequest(new { message = "Dữ liệu không hợp lệ." });
            try
            {
                Book newBook = BookFactory.CreateBook(bookType);
                newBook.Title = title; 
                newBook.Author = author; 
                newBook.BasePrice = price; 
                newBook.StockQuantity = stock;
                newBook.CategoryId = categoryId; // Gán danh mục liên kết
                newBook.ImageUrl = imageUrl;

                var allBooks = _bookService.GetAllBooks();
                newBook.Id = allBooks.Count > 0 ? allBooks.Max(b => b.Id) + 1 : 1;
                allBooks.Add(newBook);
                return Ok(new { message = "Thêm thành công", data = newBook });
            }
            catch (System.ArgumentException ex) { return BadRequest(new { message = ex.Message }); }
        }

        [HttpPut("{id}")]
        public IActionResult Update(int id, [FromQuery] string title, [FromQuery] string author, [FromQuery] decimal price, [FromQuery] int stock, [FromQuery] string imageUrl = "")
        {
            var book = _bookService.GetAllBooks().FirstOrDefault(b => b.Id == id);
            if (book == null) return NotFound(new { message = "Không tìm thấy." });
            if (!string.IsNullOrEmpty(title)) book.Title = title;
            if (price > 0) book.BasePrice = price;
            if (stock >= 0) book.StockQuantity = stock;
            if (!string.IsNullOrEmpty(imageUrl)) book.ImageUrl = imageUrl;
            return Ok(new { message = "Cập nhật thành công", data = book });
        }

        [HttpDelete("{id}")]
        public IActionResult Delete(int id)
        {
            var allBooks = _bookService.GetAllBooks();
            var book = allBooks.FirstOrDefault(b => b.Id == id);
            if (book == null) return NotFound(new { message = "Không tìm thấy." });
            allBooks.Remove(book);
            return Ok(new { message = "Xóa thành công." });
        }
    }
}