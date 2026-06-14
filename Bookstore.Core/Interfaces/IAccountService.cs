// Vị trí: Bookstore.Core/Interfaces/IAccountService.cs
using System.Collections.Generic;
using Bookstore.Core.Models.NV1_Account;

namespace Bookstore.Core.Interfaces {
    public interface IAccountService {
        List<User> GetAllAccounts();
        void CreateAccountByAdmin(User newUser);
        void UpdateAccountByAdmin(int id, string role, int points);
        void DeleteAccountByAdmin(int id);
    }
}