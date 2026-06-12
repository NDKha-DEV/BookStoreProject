using Microsoft.AspNetCore.Mvc;
using Bookstore.Core.Models;
using Bookstore.Core.Interfaces;
using Bookstore.Web.Modules.NV2_Book.Services;
using Bookstore.Web.Modules.NV2_Book.Factories;
using Bookstore.Web.Modules.NV2_Book.Strategies;
using System.Collections.Generic;
using System.Linq;

namespace Bookstore.Web.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class BookController : ControllerBase
    {
        private readonly IBookService _bookService;

        // Tiêm IBookService vào Controller thông qua Dependency Injection
        public BookController(IBookService bookService)
        {
            _bookService = bookService;
        }

        // =====================================================================
        // 1. CHỨC NĂNG XEM SẢN PHẨM (READ)
        // =====================================================================
        [HttpGet]
        public IActionResult GetAll()
        {
            var books = _bookService.GetAllBooks();
            if (books == null || books.Count == 0)
            {
                return Ok(new { message = "Chưa có sản phẩm nào trong hệ thống." });
            }
            return Ok(books);
        }

        // =====================================================================
        // 2. CHỨC NĂNG TÌM KIẾM/LỌC SẢN PHẨM (Áp dụng Strategy Pattern)
        // =====================================================================
        [HttpGet("search")]
        public IActionResult Search([FromQuery] string type, [FromQuery] string keyword)
        {
            var allBooks = _bookService.GetAllBooks();
            ISearchStrategy strategy = type?.ToLower() switch
            {
                "author" => new SearchByAuthorStrategy(),
                "title" => new SearchByTitleStrategy(),
                "type" => new SearchByTypeStrategy(),
                _ => new SearchByTitleStrategy() // Mặc định tìm theo tiêu đề sách
            };

            var searchService = new SearchService(strategy);
            var result = searchService.Search(allBooks, keyword);

            if (result == null || result.Count == 0)
            {
                return NotFound(new { message = "Không tồn tại sản phẩm" }); // Ngoại lệ E1
            }

            return Ok(result);
        }

        // =====================================================================
        // 3. CHỨC NĂNG THÊM SẢN PHẨM (CREATE - Áp dụng Factory Method)
        // =====================================================================
        [HttpPost]
        public IActionResult Create([FromQuery] string bookType, [FromQuery] string title, [FromQuery] string author, [FromQuery] decimal price, [FromQuery] int stock)
        {
            // Kiểm tra điều kiện dữ liệu đầu vào (Ngoại lệ 5a)
            if (string.IsNullOrEmpty(title) || price <= 0)
            {
                return BadRequest(new { message = "Thông tin sản phẩm bắt buộc hoặc định dạng không hợp lệ." });
            }

            try
            {
                // Sử dụng Factory để tạo đúng loại thực thể sách (Paper, Ebook, Audio)
                Book newBook = BookFactory.CreateBook(bookType);
                
                newBook.Title = title;
                newBook.Author = author;
                newBook.BasePrice = price; 
                newBook.StockQuantity = stock;

                // Lấy danh sách hiện tại ra để thêm vào và tự động tăng ID
                var allBooks = _bookService.GetAllBooks();
                newBook.Id = allBooks.Count > 0 ? allBooks.Max(b => b.Id) + 1 : 1;
                allBooks.Add(newBook);

                return Ok(new { message = "Thêm sản phẩm thành công", data = newBook });
            }
            catch (System.ArgumentException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        // =====================================================================
        // 4. CHỨC NĂNG CẬP NHẬT SẢN PHẨM (UPDATE)
        // =====================================================================
        [HttpPut("{id}")]
        public IActionResult Update(int id, [FromQuery] string title, [FromQuery] string author, [FromQuery] decimal price, [FromQuery] int stock)
        {
            var allBooks = _bookService.GetAllBooks();
            var book = allBooks.FirstOrDefault(b => b.Id == id);

            if (book == null)
            {
                return NotFound(new { message = $"Không tìm thấy sản phẩm có ID = {id} để cập nhật." });
            }

            // Tiến hành cập nhật thông tin mới nếu người dùng nhập vào URL
            if (!string.IsNullOrEmpty(title)) book.Title = title;
            if (!string.IsNullOrEmpty(author)) book.Author = author;
            if (price > 0) book.BasePrice = price;
            if (stock >= 0) book.StockQuantity = stock;

            return Ok(new { message = "Cập nhật thông tin sản phẩm thành công!", data = book });
        }

        // =====================================================================
        // 5. CHỨC NĂNG XÓA SẢN PHẨM (DELETE)
        // =====================================================================
        [HttpDelete("{id}")]
        public IActionResult Delete(int id)
        {
            var allBooks = _bookService.GetAllBooks();
            var book = allBooks.FirstOrDefault(b => b.Id == id);

            if (book == null)
            {
                return NotFound(new { message = $"Không tìm thấy sản phẩm có ID = {id} để xóa." });
            }

            // Xóa sản phẩm khỏi danh sách static lưu trong bộ nhớ tạm
            allBooks.Remove(book);

            return Ok(new { message = $"Đã xóa sản phẩm '{book.Title}' (ID: {id}) ra khỏi hệ thống." });
        }
    }
}