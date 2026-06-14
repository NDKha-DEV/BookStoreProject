// Vị trí: Bookstore.Core/Utils/PasswordHasher.cs
using System;
using System.Security.Cryptography;
using System.Text;

namespace Bookstore.Core.Utils
{
    public static class PasswordHasher
    {
        // Hàm băm mật khẩu kèm Salt ngẫu nhiên
        public static string HashPassword(string password, out string salt)
        {
            // 1. Tạo muối ngẫu nhiên ngặt bảo mật
            var saltBytes = new byte[16];
            using (var rng = RandomNumberGenerator.Create())
            {
                rng.GetBytes(saltBytes);
            }
            salt = Convert.ToBase64String(saltBytes);

            // 2. Trộn muối với mật khẩu và tiến hành băm SHA256
            using (var sha256 = SHA256.Create())
            {
                var combinedBytes = Encoding.UTF8.GetBytes(password + salt);
                var hashBytes = sha256.ComputeHash(combinedBytes);
                return Convert.ToBase64String(hashBytes);
            }
        }

        // Hàm xác minh mật khẩu nhập vào khớp với DB không
        public static bool VerifyPassword(string enteredPassword, string storedHash, string storedSalt)
        {
            using (var sha256 = SHA256.Create())
            {
                var combinedBytes = Encoding.UTF8.GetBytes(enteredPassword + storedSalt);
                var hashBytes = sha256.ComputeHash(combinedBytes);
                var enteredHash = Convert.ToBase64String(hashBytes);
                return enteredHash == storedHash;
            }
        }
    }
}