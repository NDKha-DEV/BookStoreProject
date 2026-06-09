// Trong file: Bookstore.Core/Models/Book.cs
namespace Bookstore.Core.Models
{
    public abstract class Book
    {
        public int Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public string Author { get; set; } = string.Empty;
        public decimal BasePrice { get; set; } // Giá gốc của sách
        public int StockQuantity { get; set; } // Số lượng tồn kho
        
        // Thuộc tính để nhận diện loại sách (Paper, Ebook, Audio)
        public string BookType { get; protected set; } = string.Empty;

        // Hàm abstract để các loại sách con tự định nghĩa cách hiển thị thông tin
        public abstract string GetDetails();
    }
}