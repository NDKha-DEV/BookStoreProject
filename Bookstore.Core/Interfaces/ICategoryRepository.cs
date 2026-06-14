// Vị trí: Bookstore.Core/Interfaces/ICategoryRepository.cs
using System.Collections.Generic;
using Bookstore.Core.Models.NV2_Book;

namespace Bookstore.Core.Interfaces {
    public interface ICategoryRepository {
        Category? GetById(int id);
        List<Category> GetAll();
        void Add(Category category);
        void Update(Category category);
        void Delete(int id);
    }
}