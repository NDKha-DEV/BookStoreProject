// Vị trí: Bookstore.Core/Models/MockDataStore.cs
using System;
using System.Collections.Generic;
using Bookstore.Core.Utils; 
using Bookstore.Core.Models.NV1_Account;
using Bookstore.Core.Models.NV2_Book;

namespace Bookstore.Core.Models
{
    public static class MockDataStore
    {
        public static List<User> Users { get; set; }

        static MockDataStore()
        {
            // Tiến hành băm mật khẩu mẫu tĩnh ngay khi ứng dụng khởi tạo bộ nhớ RAM
            string adminSalt, minhSalt, longSalt;
            string adminHash = PasswordHasher.HashPassword("admin123", out adminSalt);
            string minhHash = PasswordHasher.HashPassword("minh123", out minhSalt);
            string longHash = PasswordHasher.HashPassword("long123", out longSalt);

            Users = new List<User>
            {
                new User { Id = 1, Username = "admin_bookstore", PasswordHash = adminHash, PasswordSalt = adminSalt, Role = "ADMIN" },
                new User { Id = 2, Username = "minh_customer", PasswordHash = minhHash, PasswordSalt = minhSalt, Role = "CUSTOMER" },
                new User { Id = 3, Username = "long_customer", PasswordHash = longHash, PasswordSalt = longSalt, Role = "CUSTOMER" }
            };
        }

        public static List<Category> Categories { get; set; } = new List<Category>
        {
            new Category { Id = 1, Name = "Sách Công Nghệ Thông Tin", Description = "Lập trình, cơ sở dữ liệu, AI" },
            new Category { Id = 2, Name = "Sách Kinh Tế - Kinh Doanh", Description = "Kinh tế học, tài chính, quản trị" }
        };

        public static List<Book> Books { get; set; } = new List<Book>
        {
            // 🔥 SỬA LỖI: Đổi từ PhysicalBook thành PaperBook cho khớp với Factory của NV2
            new PaperBook { Id = 1, CategoryId = 1, Title = "Design Patterns cơ bản", Author = "Gang of Four", BasePrice = 150000, StockQuantity = 10 },
            new EBook { Id = 2, CategoryId = 1, Title = "C# Nâng Cao và Clean Code", Author = "Tác giả Việt", BasePrice = 100000, StockQuantity = 99 },
            new PaperBook { Id = 3, CategoryId = 2, Title = "Kinh tế học vĩ mô", Author = "Adam Smith", BasePrice = 200000, StockQuantity = 5 }
        };

        public static List<Order> Orders { get; set; } = new List<Order>();

        public static Dictionary<int, List<CartItem>> UserCarts { get; set; } = new Dictionary<int, List<CartItem>>
        {
            { 2, new List<CartItem>() },
            { 3, new List<CartItem>() }
        };
    }
}