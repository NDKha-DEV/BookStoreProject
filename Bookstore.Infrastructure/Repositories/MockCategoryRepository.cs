// Vị trí: Bookstore.Infrastructure/Repositories/MockCategoryRepository.cs
using System.Collections.Generic;
using System.Linq;
using Bookstore.Core.Interfaces;
using Bookstore.Core.Models;
using Bookstore.Core.Models.NV2_Book;

namespace Bookstore.Infrastructure.Repositories {
    public class MockCategoryRepository : ICategoryRepository {
        public Category? GetById(int id) => MockDataStore.Categories.FirstOrDefault(c => c.Id == id);
        public List<Category> GetAll() => MockDataStore.Categories;
        public void Add(Category category) => MockDataStore.Categories.Add(category);
        public void Update(Category category) { /* RAM tự cập nhật */ }
        public void Delete(int id) {
            var category = GetById(id);
            if (category != null) MockDataStore.Categories.Remove(category);
        }
    }
}