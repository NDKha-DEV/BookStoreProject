// Vị trí: Bookstore.Core/Models/Book.cs
namespace Bookstore.Core.Models
{
    public abstract class Book
    {
        public int Id { get; set; }
        public int CategoryId { get; set; } 
        public string Title { get; set; } = string.Empty;
        public string Author { get; set; } = string.Empty;
        public decimal BasePrice { get; set; } 
        public int StockQuantity { get; set; } 
        public string BookType { get; protected set; } = string.Empty;

        public abstract string GetDetails();
    }

    // ✨ ĐÃ BỔ SUNG: Thực thể Sách giấy cho Factory
    public class PhysicalBook : Book
    {
        public double WeightInGram { get; set; }

        public PhysicalBook()
        {
            BookType = "PHYSICAL";
        }

        public override string GetDetails() => $"[Sách Giấy] {Title} - Trọng lượng: {WeightInGram}g";
    }

    // ✨ ĐÃ BỔ SUNG: Thực thể EBook cho Factory
    public class EBook : Book
    {
        public double FileSizeInMb { get; set; }

        public EBook()
        {
            BookType = "EBOOK";
        }

        public override string GetDetails() => $"[EBook] {Title} - Dung lượng: {FileSizeInMb}MB";
    }
}