// vị trí: Bookstore.Core/Models/NV1_Account/User.cs
using System.Collections.Generic;

namespace Bookstore.Core.Models.NV1_Account
{
    public class User
    {
        public int Id { get; set; }
        public string Username { get; set; } = string.Empty;
        public string PasswordHash { get; set; } = string.Empty; 
        public string PasswordSalt { get; set; } = string.Empty; 
        public string Email { get; set; } = string.Empty;
        
        // Phân quyền: "Admin" hoặc "Customer" hoặc "Staff" (nhân viên)
        public string Role { get; set; } = "Customer"; 
        
        public int LoyaltyPoints { get; set; } = 0; 
    }
}