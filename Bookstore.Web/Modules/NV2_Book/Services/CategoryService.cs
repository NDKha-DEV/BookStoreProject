// Vị trí: Bookstore.Web/Modules/NV2_Book/Services/CategoryService.cs
using System.Collections.Generic;
using System.Linq;
using Bookstore.Core.Interfaces;
using Bookstore.Core.Models.NV2_Book;

namespace Bookstore.Web.Modules.NV2_Book.Services
{
    public class CategoryService : ICategoryService
    {
        private readonly ICategoryRepository _categoryRepository;

        public CategoryService(ICategoryRepository categoryRepository)
        {
            _categoryRepository = categoryRepository;
        }

        public List<Category> GetAllCategories() => _categoryRepository.GetAll();

        public Category? GetCategoryById(int id) => _categoryRepository.GetById(id);

        public bool IsNameExists(string name, int? excludeId = null)
        {
            if (string.IsNullOrWhiteSpace(name)) return false;
            return _categoryRepository.GetAll().Any(c => c.Name.ToLower() == name.Trim().ToLower() && c.Id != excludeId);
        }

        public void AddCategory(Category c)
        {
            var allCats = _categoryRepository.GetAll();
            c.Id = allCats.Count > 0 ? allCats.Max(x => x.Id) + 1 : 1;
            _categoryRepository.Add(c);
        }

        public void UpdateCategory(Category c)
        {
            var existingCategory = _categoryRepository.GetById(c.Id);
            if (existingCategory != null)
            {
                existingCategory.Name = c.Name;
                existingCategory.Description = c.Description;
                _categoryRepository.Update(existingCategory);
            }
        }

        public void DeleteCategory(int id)
        {
            _categoryRepository.Delete(id);
        }
    }
}