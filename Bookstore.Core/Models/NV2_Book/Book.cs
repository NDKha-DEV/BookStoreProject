// vị trí: Bookstore.Core/Models/NV2_Book/Book.cs
namespace Bookstore.Core.Models.NV2_Book
{
    public abstract class Book
    {
        public int Id { get; set; }
        public int CategoryId { get; set; } // ✨ BỔ SUNG: Khớp nối danh mục với MockDataStore
        public string Title { get; set; } = string.Empty;
        public string Author { get; set; } = string.Empty;
        public decimal BasePrice { get; set; } 
        public int StockQuantity { get; set; } 
        public string BookType { get; protected set; } = string.Empty;

        public abstract string GetDetails();
    }

    public class PaperBook : Book 
    { 
        public PaperBook() => BookType = "Paper"; 
        public override string GetDetails() => $"[Paper Book] {Title} - Author: {Author}, Price: {BasePrice}";
    }

    public class EBook : Book 
    { 
        public EBook() => BookType = "EBook"; 
        public override string GetDetails() => $"[E-Book] {Title} - Author: {Author}, Price: {BasePrice}";
    }

    public class AudioBook : Book 
    { 
        public AudioBook() => BookType = "Audio"; 
        public override string GetDetails() => $"[Audio Book] {Title} - Author: {Author}, Price: {BasePrice}";
    }

    public static class BookFactory
    {
        public static Book CreateBook(string type)
        {
            return type?.ToLower() switch
            {
                "paper" => new PaperBook(),
                "ebook" => new EBook(),
                "audio" => new AudioBook(),
                _ => throw new System.ArgumentException($"Loại sách '{type}' không hợp lệ.")
            };
        }
    }
}