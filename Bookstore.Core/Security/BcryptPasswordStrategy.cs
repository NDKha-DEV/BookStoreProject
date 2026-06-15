// Vị trí: Bookstore.Core/Security/BcryptPasswordStrategy.cs
namespace Bookstore.Core.Security
{
    public class BcryptPasswordStrategy : IPasswordHashStrategy
    {
        public (string Hash, string Salt) HashPassword(string password)
        {
            // BCrypt tự động sinh và nhúng salt vào trong chuỗi hash
            string salt = BCrypt.Net.BCrypt.GenerateSalt();
            string hash = BCrypt.Net.BCrypt.HashPassword(password, salt);
            
            return (hash, salt);
        }

        public bool VerifyPassword(string password, string hash, string salt)
        {
            return BCrypt.Net.BCrypt.Verify(password, hash);
        }
    }
}