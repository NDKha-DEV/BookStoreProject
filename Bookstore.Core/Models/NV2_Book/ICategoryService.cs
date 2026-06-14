// Vị trí: Bookstore.Core/Models/NV2_Book/ICategoryService.cs
using System.Collections.Generic;

namespace Bookstore.Core.Models.NV2_Book
{
    public interface ICategoryService
    {
        List<Category> GetAllCategories();
        Category? GetCategoryById(int id);
        bool IsNameExists(string name, int? excludeId = null);
        void AddCategory(Category c);
        void UpdateCategory(Category c);
        void DeleteCategory(int id);
    }
}