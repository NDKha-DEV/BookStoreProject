namespace Bookstore.Core.Models
{
    // 1. Model Danh mục
    public class Category
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
    }

    // LƯU Ý: ĐÃ XÓA LỚP 'Book' GỐC Ở ĐÂY ĐỂ TRÁNH LỖI TRÙNG LẶP CS0101, 
    // HỆ THỐNG SẼ TỰ ĐỘNG DÙNG LỚP 'Book' XỊN SẴN CÓ CỦA DỰ ÁN.

    // 2. Các loại sách cụ thể (Bổ sung hàm GetDetails() để sửa lỗi CS0534)
    public class PaperBook : Book 
    { 
        public PaperBook() => BookType = "Paper"; 
        
        // Cần có hàm này để thỏa mãn lớp abstract gốc
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

    // 3. Factory Method sinh sách
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