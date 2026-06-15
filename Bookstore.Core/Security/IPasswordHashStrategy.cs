// Vị trí: Bookstore.Core/Security/IPasswordHashStrategy.cs
namespace Bookstore.Core.Security
{
    public interface IPasswordHashStrategy
    {
        // Trả về Tuple gồm Hash và Salt
        (string Hash, string Salt) HashPassword(string password);
        
        // Kiểm tra mật khẩu
        bool VerifyPassword(string password, string hash, string salt);
    }
}