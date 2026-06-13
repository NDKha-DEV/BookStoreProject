using System;
using System.Collections.Generic;

namespace Bookstore.Core.Models
{
    /// <summary>
    /// Cơ sở dữ liệu giả lập trong RAM (In-Memory Database) 
    /// Giúp cả 5 thành viên lấy dữ liệu dùng chung và kiểm thử luồng chạy độc lập.
    /// </summary>
    public static class MockDataStore
    {
        // 1. Dữ liệu mẫu cho NV1 (Tài khoản & Phân quyền)
        public static List<User> Users = new List<User>
        {
            new User { Id = 1, Username = "admin_bookstore", PasswordHash = "admin123", Email = "admin@tluy.edu.vn", Role = "Admin" },
            new User { Id = 2, Username = "minh_customer", PasswordHash = "customer123", Email = "minh@gmail.com", Role = "Customer", LoyaltyPoints = 10 },
            new User { Id = 3, Username = "long_customer", PasswordHash = "customer123", Email = "long@gmail.com", Role = "Customer", LoyaltyPoints = 150 }
        };

        // 2. Dữ liệu mẫu cho NV2 (Sách - Áp dụng cấu trúc kế thừa từ lớp trừu tượng Book)
        public static List<Book> Books = new List<Book>
        {
            new PhysicalBook { Id = 1, Title = "Giáo trình Mẫu Thiết Kế (Design Patterns)", Author = "NXB Đại Học", BasePrice = 120000, StockQuantity = 50, WeightInGram = 450 },
            new PhysicalBook { Id = 2, Title = "Lập trình C# nâng cao từ cơ bản", Author = "Nguyễn Văn A", BasePrice = 180000, StockQuantity = 20, WeightInGram = 600 },
            new EBook { Id = 3, Title = "Kỷ nguyên AI và Công nghệ thông tin 2026", Author = "John Doe", BasePrice = 75000, StockQuantity = 9999, FileSizeInMb = 14.5 },
            new EBook { Id = 4, Title = "Kiến trúc phần mềm phân tầng (.NET Core)", Author = "Trần Văn B", BasePrice = 90000, StockQuantity = 9999, FileSizeInMb = 8.2 }
        };

        // 3. Dữ liệu mẫu cho NV3 (Giỏ hàng - Mặc định khởi tạo mỗi khách một giỏ trống)
        public static Dictionary<int, Cart> UserCarts = new Dictionary<int, Cart>
        {
            { 2, new Cart { DeliveryDistanceInKm = 5.2m } }, // Giỏ hàng của khách hàng Minh (Cách cửa hàng 5.2 km)
            { 3, new Cart { DeliveryDistanceInKm = 12.0m } } // Giỏ hàng của khách hàng Long (Cách cửa hàng 12 km)
        };

        // 4. Dữ liệu mẫu cho NV4 & NV5 (Lịch sử Đơn hàng & Hóa đơn thanh toán)
        // Ban đầu để trống, khi chạy chức năng Checkout/Payment thì code sẽ tự động Add vào đây
        public static List<Order> Orders = new List<Order>();
        public static List<Payment> Payments = new List<Payment>();
    }

    // =========================================================================
    // LỚP CON TẠM THỜI CỦA SÁCH (Phục vụ Factory Method của NV2)
    // Các lớp này kế thừa lớp abstract Book từ tầng Core của bạn
    // =========================================================================

    public class PhysicalBook : Book
    {
        public int WeightInGram { get; set; }
        public PhysicalBook() => BookType = "Physical";
        public override string GetDetails() => $"[Sách Giấy] {Title} - Tác giả: {Author} (Nặng: {WeightInGram}g) - Giá: {BasePrice:N0} VNĐ - Tồn kho: {StockQuantity}";
    }

    public class EBook : Book
    {
        public double FileSizeInMb { get; set; }
        public EBook() => BookType = "EBook";
        public override string GetDetails() => $"[Sách Điện Tử] {Title} - Tác giả: {Author} (Dung lượng: {FileSizeInMb}MB) - Giá: {BasePrice:N0} VNĐ";
    }
}