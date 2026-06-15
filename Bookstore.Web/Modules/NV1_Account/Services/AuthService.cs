using System;
using System.Linq;
using Bookstore.Core.Models.NV1_Account;
using Bookstore.Core.Utils;
using Bookstore.Core.Security;
using Bookstore.Core.Models; // Để đọc dữ liệu MockDataStore

namespace Bookstore.Web.Modules.NV1_Account.Services
{
    public class AuthService : IAuthService
    {
        private readonly IPasswordHashStrategy _passwordStrategy;

        // Dependency Injection (Tiêm Strategy vào)
        public AuthService(IPasswordHashStrategy passwordStrategy)
        {
            _passwordStrategy = passwordStrategy;
        }

        public User? CurrentLoggedInUser { get; private set; }

        public bool Login(string username, string password)
        {
            var user = MockDataStore.Users.FirstOrDefault(u => u.Username.Equals(username, StringComparison.OrdinalIgnoreCase));
            if (user == null) return false;

            bool isValid = _passwordStrategy.VerifyPassword(password, user.PasswordHash, user.PasswordSalt);
            if (isValid)
            {
                CurrentLoggedInUser = user;
                return true;
            }
            return false;
        }

        public void Logout() => CurrentLoggedInUser = null;

        public bool Register(string username, string password)
        {
            if (MockDataStore.Users.Any(u => u.Username.Equals(username, StringComparison.OrdinalIgnoreCase)))
                return false;

            var (hash, salt) = _passwordStrategy.HashPassword(password);

            var newUser = new User
            {
                Id = MockDataStore.Users.Count + 1,
                Username = username,
                PasswordHash = hash,
                PasswordSalt = salt,
                Role = "Customer"
            };
            MockDataStore.Users.Add(newUser);
            return true;
        }

        public bool UpdateProfile(string fullName, string address, string? newPassword)
        {
            if (CurrentLoggedInUser == null) return false;
            
            var user = MockDataStore.Users.FirstOrDefault(u => u.Id == CurrentLoggedInUser.Id);
            if (user != null)
            {
                // Cập nhật thông tin cơ bản
                user.FullName = fullName;
                user.Address = address;

                // Chỉ cập nhật mật khẩu nếu người dùng có gửi password mới lên
                if (!string.IsNullOrWhiteSpace(newPassword))
                {
                    var (hash, salt) = _passwordStrategy.HashPassword(newPassword);
                    user.PasswordHash = hash;
                    user.PasswordSalt = salt;
                }
                
                return true;
            }
            return false;
        }
        public string GetCurrentUserRole()
        {
            return CurrentLoggedInUser?.Role ?? "Guest";
        }
    }
}