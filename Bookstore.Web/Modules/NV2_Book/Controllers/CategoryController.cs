// vị trí: Bookstore.Web/Modules/NV2_Book/Controllers/CategoryController.cs
using Microsoft.AspNetCore.Mvc;
using Bookstore.Core.Models.NV2_Book;
using Bookstore.Web.Modules.NV2_Book.Services;

namespace Bookstore.Web.Modules.NV2_Book.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class CategoryController : ControllerBase
    {
        private readonly CategoryService _categoryService;
        public CategoryController(CategoryService catService) 
        { 
            _categoryService = catService; 
        }

        [HttpGet]
        public IActionResult GetAll() => Ok(_categoryService.GetAllCategories());

        [HttpPost]
        public IActionResult Create([FromQuery] string name, [FromQuery] string description)
        {
            if (_categoryService.IsNameExists(name)) return BadRequest(new { message = "Tên danh mục này đã tồn tại, vui lòng chọn tên khác" });
            var newCat = new Category { Name = name, Description = description };
            _categoryService.AddCategory(newCat);
            return Ok(new { message = "Thêm danh mục thành công", data = newCat });
        }

        [HttpPut("{id}")]
        public IActionResult Update(int id, [FromQuery] string name, [FromQuery] string description)
        {
            var category = _categoryService.GetCategoryById(id);
            if (category == null) return NotFound(new { message = "Không tìm thấy." });
            if (_categoryService.IsNameExists(name, id)) return BadRequest(new { message = "Tên danh mục mới bị trùng lặp." });
            if (!string.IsNullOrWhiteSpace(name)) category.Name = name;
            if (!string.IsNullOrWhiteSpace(description)) category.Description = description;
            _categoryService.UpdateCategory(category);
            return Ok(new { message = "Cập nhật thành công", data = category });
        }

        [HttpDelete("{id}")]
        public IActionResult Delete(int id)
        {
            if (id == 1 || id == 2) return BadRequest(new { message = "Không thể xóa do danh mục này đang chứa sản phẩm hệ thống." });
            _categoryService.DeleteCategory(id);
            return Ok(new { message = "Xóa danh mục thành công." });
        }
    }
}