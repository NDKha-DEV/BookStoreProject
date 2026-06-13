// Vị trí: Bookstore.Web/Modules/NV1_Account/AuthService.cs
using Bookstore.Core.Models;
using Bookstore.Core.Utils;
using System.Linq;

namespace Bookstore.Web.Modules.NV1_Account
{
    public class AuthService
    {
        private static AuthService? _instance;
        private static readonly object _lock = new object();
        public User? CurrentLoggedInUser { get; private set; }

        private AuthService() { }

        public static AuthService Instance
        {
            get
            {
                lock (_lock)
                {
                    if (_instance == null) _instance = new AuthService();
                    return _instance;
                }
            }
        }

        public bool Login(string username, string password)
        {
            var user = MockDataStore.Users.FirstOrDefault(u => u.Username.Equals(username, StringComparison.OrdinalIgnoreCase));
            if (user == null) return false;

            // ✨ Sử dụng Helper để xác minh chuỗi băm bảo mật
            bool isValid = PasswordHasher.VerifyPassword(password, user.PasswordHash, user.PasswordSalt);
            if (isValid)
            {
                CurrentLoggedInUser = user;
                return true;
            }
            return false;
        }

        public void Logout() => CurrentLoggedInUser = null;

        // ✨ BỔ SUNG: Đăng ký tài khoản (Chỉ dành cho khách hàng)
        public bool Register(string username, string password)
        {
            if (MockDataStore.Users.Any(u => u.Username.Equals(username, StringComparison.OrdinalIgnoreCase)))
                return false;

            // ✨ Khi đăng ký tài khoản mới: Băm mật khẩu ra rồi mới lưu vào bộ nhớ RAM
            string hash = PasswordHasher.HashPassword(password, out string salt);
            
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

        // ✨ BỔ SUNG: Khách hàng tự cập nhật thông tin cá nhân của mình
        public bool UpdateProfile(string newPassword)
        {
            if (CurrentLoggedInUser == null) return false;
            var user = MockDataStore.Users.FirstOrDefault(u => u.Id == CurrentLoggedInUser.Id);
            if (user != null)
            {
                user.PasswordHash = newPassword;
                return true;
            }
            return false;
        }
    }
}