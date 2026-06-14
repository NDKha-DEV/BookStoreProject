// Vị trí: Bookstore.Infrastructure/Repositories/MockUserRepository.cs
using System.Collections.Generic;
using System.Linq;
using Bookstore.Core.Interfaces;
using Bookstore.Core.Models;
using Bookstore.Core.Models.NV1_Account;

namespace Bookstore.Infrastructure.Repositories {
    public class MockUserRepository : IUserRepository {
        public User? GetById(int id) => MockDataStore.Users.FirstOrDefault(u => u.Id == id);
        public User? GetByUsername(string username) => MockDataStore.Users.FirstOrDefault(u => u.Username.Equals(username, System.StringComparison.OrdinalIgnoreCase));
        public List<User> GetAll() => MockDataStore.Users; // 🌟 Thực thi
        public void Add(User user) => MockDataStore.Users.Add(user);
        
        public void Update(User user) {
            // RAM tự cập nhật theo tham chiếu object
        }

        public void Delete(int id) { // 🌟 Thực thi
            var user = GetById(id);
            if (user != null) MockDataStore.Users.Remove(user);
        }
    }
}