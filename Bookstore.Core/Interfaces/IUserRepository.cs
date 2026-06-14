// Vị trí: Bookstore.Core/Interfaces/IUserRepository.cs
using System.Collections.Generic;
using Bookstore.Core.Models.NV1_Account;

namespace Bookstore.Core.Interfaces {
    public interface IUserRepository {
        User? GetById(int id);
        User? GetByUsername(string username);
        List<User> GetAll(); 
        void Add(User user);
        void Update(User user); 
        void Delete(int id);
    }
}