// Trong file: Bookstore.Core/Models/User.cs
namespace Bookstore.Core.Models
{
    public class User
    {
        public int Id { get; set; }
        public string Username { get; set; } = string.Empty;
        public string PasswordHash { get; set; } = string.Empty; // Lưu mật khẩu đã mã hóa
        public string Email { get; set; } = string.Empty;
        
        // Phân quyền: "Admin" hoặc "Customer"
        // Quyết định việc NV1_Account chặn hay cho qua bằng Proxy Pattern
        public string Role { get; set; } = "Customer"; 
        
        public int LoyaltyPoints { get; set; } = 0; // Điểm tích lũy (để NV4_Order Observer cộng điểm)
    }
}