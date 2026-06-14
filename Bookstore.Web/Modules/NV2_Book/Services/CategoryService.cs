using System.Collections.Generic;
using System.Linq;
using Bookstore.Core.Models;
using Bookstore.Core.Models.NV2_Book;

namespace Bookstore.Web.Modules.NV2_Book.Services
{
    public class CategoryService
    {
        // 🔥 ĐỒNG BỘ: Chuyển toàn bộ luồng đọc/ghi sang kho trung tâm của dự án
        public List<Category> GetAllCategories() => MockDataStore.Categories;

        public Category? GetCategoryById(int id) => MockDataStore.Categories.FirstOrDefault(c => c.Id == id);

        public bool IsNameExists(string name, int? excludeId = null)
        {
            if (string.IsNullOrWhiteSpace(name)) return false;
            return MockDataStore.Categories.Any(c => c.Name.ToLower() == name.Trim().ToLower() && c.Id != excludeId);
        }

        public void AddCategory(Category c)
        {
            c.Id = MockDataStore.Categories.Count > 0 ? MockDataStore.Categories.Max(x => x.Id) + 1 : 1;
            MockDataStore.Categories.Add(c);
        }

        public void UpdateCategory(Category c)
        {
            var existingCategory = GetCategoryById(c.Id);
            if (existingCategory != null)
            {
                existingCategory.Name = c.Name;
                existingCategory.Description = c.Description;
            }
        }

        public void DeleteCategory(int id)
        {
            var category = GetCategoryById(id);
            if (category != null)
            {
                MockDataStore.Categories.Remove(category);
            }
        }
    }
}