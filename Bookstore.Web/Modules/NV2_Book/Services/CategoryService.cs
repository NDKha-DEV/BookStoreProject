using Bookstore.Core.Models;
using System.Collections.Generic;
using System.Linq;

namespace Bookstore.Web.Modules.NV2_Book.Services
{
    public class CategoryService
    {
        // Mock data static danh mục ban đầu
        private static readonly List<Category> _categories = new List<Category>
        {
            new Category { Id = 1, Name = "Sách Văn Học", Description = "Các tác phẩm tiểu thuyết, truyện ngắn" },
            new Category { Id = 2, Name = "Sách Công Nghệ", Description = "Tài liệu lập trình, kỹ thuật phần mềm" },
            new Category { Id = 3, Name = "Sách Kỹ Năng", Description = "Phát triển bản thân, tư duy kinh doanh" }
        };

        public List<Category> GetAllCategories() => _categories;

        public Category? GetCategoryById(int id) => _categories.FirstOrDefault(c => c.Id == id);

        public bool IsNameExists(string name, int? excludeId = null)
        {
            return _categories.Any(c => c.Name.ToLower() == name.Trim().ToLower() && c.Id != excludeId);
        }

        public void AddCategory(Category category)
        {
            category.Id = _categories.Count > 0 ? _categories.Max(c => c.Id) + 1 : 1;
            _categories.Add(category);
        }

        public void UpdateCategory(Category updatedCategory)
        {
            var existing = GetCategoryById(updatedCategory.Id);
            if (existing != null)
            {
                existing.Name = updatedCategory.Name;
                existing.Description = updatedCategory.Description;
            }
        }

        public void DeleteCategory(int id)
        {
            var category = GetCategoryById(id);
            if (category != null)
            {
                _categories.Remove(category);
            }
        }
    }
}