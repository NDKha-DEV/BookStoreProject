using Bookstore.Core.Models;

namespace Bookstore.Core.Models
{
    public class PaperBook : Book
    {
        public PaperBook()
        {
            // Xác định đây là sách giấy
            BookType = "Paper";
        }

        public override string GetDetails()
        {
            return $"[Sách giấy] {Title} - {Author}";
        }
    }
}