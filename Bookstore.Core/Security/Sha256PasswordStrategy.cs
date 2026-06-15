// Vị trí: Bookstore.Core/Security/Sha256PasswordStrategy.cs
using System.Security.Cryptography;
using System.Text;
using System;

namespace Bookstore.Core.Security
{
    public class Sha256PasswordStrategy : IPasswordHashStrategy
    {
        public (string Hash, string Salt) HashPassword(string password)
        {
            // Logic băm SHA256 (Tạo ngẫu nhiên một chuỗi Salt)
            string salt = Guid.NewGuid().ToString(); 
            string hash = ComputeSha256(password + salt);
            return (hash, salt);
        }

        public bool VerifyPassword(string password, string hash, string salt)
        {
            string hashToVerify = ComputeSha256(password + salt);
            return hashToVerify == hash;
        }

        private string ComputeSha256(string input)
        {
            using (var sha256 = SHA256.Create())
            {
                var bytes = Encoding.UTF8.GetBytes(input);
                var hashBytes = sha256.ComputeHash(bytes);
                return Convert.ToBase64String(hashBytes);
            }
        }
    }
}