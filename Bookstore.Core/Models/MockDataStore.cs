using System;
using System.Collections.Generic;
using Bookstore.Core.Utils; 
using Bookstore.Core.Models.NV1_Account;
using Bookstore.Core.Models.NV2_Book;
using Bookstore.Core.Models.NV3_Cart;
using Bookstore.Core.Models.NV4_Order;

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

            // ✨ ĐỒNG BỘ: Bổ sung dữ liệu FullName và Address mẫu cho người dùng
            Users = new List<User>
            {
                new User { Id = 1, Username = "admin_bookstore", PasswordHash = adminHash, PasswordSalt = adminSalt, Role = "ADMIN", FullName = "Quản Trị Viên Hệ Thống", Address = "Trụ sở chính Bookstore, Hà Nội" },
                new User { Id = 2, Username = "minh_customer", PasswordHash = minhHash, PasswordSalt = minhSalt, Role = "CUSTOMER", FullName = "Nguyễn Văn Minh", Address = "123 Đường Lê Lợi, Quận 1, TP. HCM" },
                new User { Id = 3, Username = "long_customer", PasswordHash = longHash, PasswordSalt = longSalt, Role = "CUSTOMER", FullName = "Trần Hoàng Long", Address = "456 Đường Nguyễn Trãi, Thanh Xuân, Hà Nội" }
            };
        }

        public static List<Category> Categories { get; set; } = new List<Category>
        {
            new Category { Id = 1, Name = "Sách Công Nghệ Thông Tin", Description = "Lập trình, cơ sở dữ liệu, AI" },
            new Category { Id = 2, Name = "Sách Kinh Tế - Kinh Doanh", Description = "Kinh tế học, tài chính, quản trị" }
        };

        public static List<Book> Books { get; set; } = new List<Book>
        {
            // ✨ BỔ SUNG: Khởi tạo đường dẫn ImageUrl mẫu cho từng đầu sách (Có thể dùng link ảnh thật hoặc link placeholder để test)
            new PaperBook { 
                Id = 1, 
                CategoryId = 1, 
                Title = "Design Patterns cơ bản", 
                Author = "Gang of Four", 
                BasePrice = 150000, 
                StockQuantity = 10,
                ImageUrl = "https://images.unsplash.com/photo-1532012197267-da84d127e765?w=500" // Link ảnh minh họa mẫu
            },
            new EBook { 
                Id = 2, 
                CategoryId = 1, 
                Title = "C# Nâng Cao và Clean Code", 
                Author = "Tác giả Việt", 
                BasePrice = 100000, 
                StockQuantity = 99,
                ImageUrl = "https://images.unsplash.com/photo-1516979187457-637abb4f9353?w=500"
            },
            new PaperBook { 
                Id = 3, 
                CategoryId = 2, 
                Title = "Kinh tế học vĩ mô", 
                Author = "Adam Smith", 
                BasePrice = 200000, 
                StockQuantity = 5,
                ImageUrl = "https://images.unsplash.com/photo-1506880018603-83d5b814b5a6?w=500"
            }
        };

        public static List<Order> Orders { get; set; } = new List<Order>();

        public static Dictionary<int, Cart> UserCarts { get; set; } = new Dictionary<int, Cart>
        {
            { 2, new Cart() },
            { 3, new Cart() }
        };

        public static List<Payment> Payments { get; set; } = new List<Payment>();
    }
}