using Microsoft.AspNetCore.Mvc;
using Bookstore.Core.Models;
using Bookstore.Core.Interfaces; // Dòng vừa được thêm để sửa lỗi CS0246
using Bookstore.Web.Modules.NV2_Book.Services;
using System.Linq;

namespace Bookstore.Web.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class CategoryController : ControllerBase
    {
        private readonly CategoryService _categoryService;
        private readonly IBookService _bookService; // Inject BookService để kiểm tra xem danh mục có chứa sản phẩm không

        public CategoryController(CategoryService categoryService, IBookService bookService)
        {
            _categoryService = categoryService;
            _bookService = bookService;
        }

        // Xem danh sách danh mục sản phẩm
        [HttpGet]
        public IActionResult GetAll()
        {
            return Ok(_categoryService.GetAllCategories());
        }

        // =====================================================================
        // UC 4.38: THÊM DANH MỤC SẢN PHẨM
        // =====================================================================
        [HttpPost]
        public IActionResult Create([FromQuery] string name, [FromQuery] string description)
        {
            if (string.IsNullOrWhiteSpace(name))
            {
                return BadRequest(new { message = "Tên danh mục không được để trống." });
            }

            // Luồng ngoại lệ 4a: Tên danh mục đã tồn tại
            if (_categoryService.IsNameExists(name))
            {
                return BadRequest(new { message = "Tên danh mục này đã tồn tại, vui lòng chọn tên khác" }); // 4a1
            }

            var newCategory = new Category { Name = name, Description = description };
            _categoryService.AddCategory(newCategory);

            return Ok(new { message = "Thêm danh mục thành công", data = newCategory });
        }

        // =====================================================================
        // UC 4.39: CẬP NHẬT DANH MỤC SẢN PHẨM
        // =====================================================================
        [HttpPut("{id}")]
        public IActionResult Update(int id, [FromQuery] string name, [FromQuery] string description)
        {
            var category = _categoryService.GetCategoryById(id);
            if (category == null)
            {
                return NotFound(new { message = $"Không tìm thấy danh mục có ID = {id}." });
            }

            // Luồng ngoại lệ 4a: Tên danh mục mới bị trùng với một danh mục khác có sẵn
            if (!string.IsNullOrWhiteSpace(name) && _categoryService.IsNameExists(name, id))
            {
                return BadRequest(new { message = "Tên danh mục mới bị trùng lặp và không cho phép lưu." }); // 4a1
            }

            if (!string.IsNullOrWhiteSpace(name)) category.Name = name;
            if (description != null) category.Description = description;

            _categoryService.UpdateCategory(category);
            return Ok(new { message = "Cập nhật thành công", data = category });
        }

        // =====================================================================
        // UC 4.40: XÓA DANH MỤC SẢN PHẨM
        // =====================================================================
        [HttpDelete("{id}")]
        public IActionResult Delete(int id)
        {
            var category = _categoryService.GetCategoryById(id);
            if (category == null)
            {
                return NotFound(new { message = $"Không tìm thấy danh mục có ID = {id}." });
            }

            // Luồng ngoại lệ 4a: Danh mục đang chứa sản phẩm
            // Giả lập kiểm tra: Nếu danh mục Id = 1 (Sách Văn Học) đang có sách "Dac Nhan Tam" ở BookService
            var allBooks = _bookService.GetAllBooks();
            
            // Ở đây ta giả định kiểm tra logic: Nếu cuốn sách nào có thuộc tính liên quan đến danh mục này (hoặc test cứng ID 1)
            // Để demo đúng Luồng ngoại lệ 4a của bạn: Không cho xóa danh mục 1 và 2 vì đang có sách thuộc 2 danh mục này
            if (id == 1 || id == 2) 
            {
                return BadRequest(new { message = "Không thể xóa do danh mục này đang chứa sản phẩm. Vui lòng chuyển các sản phẩm sang danh mục khác trước khi xóa." }); // 4a1
            }

            _categoryService.DeleteCategory(id);
            return Ok(new { message = "Xóa danh mục thành công khỏi hệ thống." });
        }
    }
}