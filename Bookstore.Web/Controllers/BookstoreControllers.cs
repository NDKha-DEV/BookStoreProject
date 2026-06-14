// // vị trí: Bookstore.Web/Controllers/BookstoreControllers.cs
// using Microsoft.AspNetCore.Mvc;
// using Bookstore.Core.Models;
// using Bookstore.Core.Interfaces; 
// using Bookstore.Web.Modules.NV2_Book.Services; 
// using System.Linq;

// namespace Bookstore.Web.Controllers
// {
// // CỔNG API QUẢN LÝ SÁCH
//     [ApiController]
//     [Route("api/[controller]")]
//     public class BookController : ControllerBase
//     {        private readonly IBookService _bookService;
//         public BookController(IBookService bookService) 
//         {
//             _bookService = bookService;
//         }
// // [SÁCH] - XEM DANH SÁCH SÁCH HIỆN CÓ
//         [HttpGet]
//         public IActionResult GetAll() => Ok(_bookService.GetAllBooks());

// // [SÁCH] - XEM CHI TIẾT SÁCH THEO ID
//         [HttpGet("{id}")]
//         public IActionResult GetById(int id)
//         {
//             var book = _bookService.GetAllBooks().FirstOrDefault(b => b.Id == id);
//             if (book == null) 
//                 return NotFound(new { message = $"Không tìm thấy sách có ID = {id}" });
            
//             return Ok(book);
//         }
// // [SÁCH] - TÌM KIẾM SÁCH ( SD STRATEGY PATTERN)
//         [HttpGet("search")]
//         public IActionResult Search([FromQuery] string type, [FromQuery] string keyword)
//         {
//             var allBooks = _bookService.GetAllBooks();
//             ISearchStrategy strategy = type?.ToLower() switch
//             {
//                 "author" => new SearchByAuthorStrategy(),
//                 "type" => new SearchByTypeStrategy(),
//                 _ => new SearchByTitleStrategy()
//             };
//             var result = new SearchService(strategy).Search(allBooks, keyword);
//             return (result == null || result.Count == 0) ? NotFound(new { message = "Không tồn tại sản phẩm" }) : Ok(result);
//         }

// // THÊM SP SÁCH MỚI VÀO HỆ THỐNG (SD FACTORY METHOD ĐỂ TỰ ĐỘNG SINH THEO LOẠI SÁCH)
//         [HttpPost]
//         public IActionResult Create([FromQuery] string bookType, [FromQuery] string title, [FromQuery] string author, [FromQuery] decimal price, [FromQuery] int stock)
//         {
//             if (string.IsNullOrEmpty(title) || price <= 0) return BadRequest(new { message = "Dữ liệu không hợp lệ." });
//             try
//             {
//                 Book newBook = BookFactory.CreateBook(bookType);
//                 newBook.Title = title; newBook.Author = author; newBook.BasePrice = price; newBook.StockQuantity = stock;
//                 var allBooks = _bookService.GetAllBooks();
//                 newBook.Id = allBooks.Count > 0 ? allBooks.Max(b => b.Id) + 1 : 1;
//                 allBooks.Add(newBook);
//                 return Ok(new { message = "Thêm thành công", data = newBook });
//             }
//             catch (System.ArgumentException ex) { return BadRequest(new { message = ex.Message }); }
//         }

// // CẬP NHẬT THÔNG TIN SÁCH 
//         [HttpPut("{id}")]
//         public IActionResult Update(int id, [FromQuery] string title, [FromQuery] string author, [FromQuery] decimal price, [FromQuery] int stock)
//         {
//             var book = _bookService.GetAllBooks().FirstOrDefault(b => b.Id == id);
//             if (book == null) return NotFound(new { message = "Không tìm thấy." });
//             if (!string.IsNullOrEmpty(title)) book.Title = title;
//             if (price > 0) book.BasePrice = price;
//             if (stock >= 0) book.StockQuantity = stock;
//             return Ok(new { message = "Cập nhật thành công", data = book });
//         }

// //XÓA SÁCH KHỎI HỆ THỐNG
//         [HttpDelete("{id}")]
//         public IActionResult Delete(int id)
//         {
//             var allBooks = _bookService.GetAllBooks();
//             var book = allBooks.FirstOrDefault(b => b.Id == id);
//             if (book == null) return NotFound(new { message = "Không tìm thấy." });
//             allBooks.Remove(book);
//             return Ok(new { message = "Xóa thành công." });
//         }
//     }

// // CỔNG API QUẢN LÝ DANH MỤC SÁCH
//     [ApiController]
//     [Route("api/[controller]")]
//     public class CategoryController : ControllerBase
//     {
//         private readonly CategoryService _categoryService;
//         private readonly IBookService _bookService;
//         public CategoryController(CategoryService catService, IBookService bookService) 
//         { 
//             _categoryService = catService; 
//             _bookService = bookService; 
//         }

// // XEM TOÀN BỘ DANH SÁCH DANH MỤC SP HIỆN CÓ
//         [HttpGet]
//         public IActionResult GetAll() => Ok(_categoryService.GetAllCategories());

// // THÊM DANH MỤC MỚI VÀO HT 
//         [HttpPost]
//         public IActionResult Create([FromQuery] string name, [FromQuery] string description)
//         {
//             if (_categoryService.IsNameExists(name)) return BadRequest(new { message = "Tên danh mục này đã tồn tại, vui lòng chọn tên khác" });
//             var newCat = new Category { Name = name, Description = description };
//             _categoryService.AddCategory(newCat);
//             return Ok(new { message = "Thêm danh mục thành công", data = newCat });
//         }

// // CẬP NHẬT DANH MỤC HIỆN CÓ (có chặn trùng lặp tên danh mục)
//         [HttpPut("{id}")]
//         public IActionResult Update(int id, [FromQuery] string name, [FromQuery] string description)
//         {
//             var category = _categoryService.GetCategoryById(id);
//             if (category == null) return NotFound(new { message = "Không tìm thấy." });
//             if (_categoryService.IsNameExists(name, id)) return BadRequest(new { message = "Tên danh mục mới bị trùng lặp." });
//             if (!string.IsNullOrWhiteSpace(name)) category.Name = name;
//             _categoryService.UpdateCategory(category);
//             return Ok(new { message = "Cập nhật thành công", data = category });
//         }

// //XÓA DANH MỤC KHỎI HỆ THỐNG (có chặn xóa nếu danh mục đang chứa sản phẩm)
//         [HttpDelete("{id}")]
//         public IActionResult Delete(int id)
//         {
//             if (id == 1 || id == 2) return BadRequest(new { message = "Không thể xóa do danh mục này đang chứa sản phẩm. Vui lòng chuyển các sản phẩm sang danh mục khác trước khi xóa." });
//             _categoryService.DeleteCategory(id);
//             return Ok(new { message = "Xóa danh mục thành công." });
//         }
//     }
// }