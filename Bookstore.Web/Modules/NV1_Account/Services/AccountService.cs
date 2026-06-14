// Vị trí: Bookstore.Web/Modules/NV1_Account/AccountService.cs
using System.Collections.Generic;
using System.Linq;
using Bookstore.Core.Models;
using Bookstore.Core.Models.NV1_Account;

namespace Bookstore.Web.Modules.NV1_Account.Services
{
    public class AccountService
    {
        // ✨ BỔ SUNG: CRUD danh sách tài khoản (Hàm bảo mật chỉ dành cho Admin điều hành)
        public List<User> GetAllAccounts() => MockDataStore.Users;

        public void CreateAccountByAdmin(User newUser)
        {
            newUser.Id = MockDataStore.Users.Count + 1;
            MockDataStore.Users.Add(newUser);
            if (!MockDataStore.UserCarts.ContainsKey(newUser.Id))
                MockDataStore.UserCarts[newUser.Id] = new List<CartItem>();
        }

        public void UpdateAccountByAdmin(int id, string role, int points)
        {
            var user = MockDataStore.Users.FirstOrDefault(u => u.Id == id);
            if (user != null)
            {
                user.Role = role;
                user.LoyaltyPoints = points;
            }
        }

        public void DeleteAccountByAdmin(int id)
        {
            var user = MockDataStore.Users.FirstOrDefault(u => u.Id == id);
            if (user != null) MockDataStore.Users.Remove(user);
        }
    }
}