using Bookstore.Core.Models;

namespace Bookstore.Core.Models
{
    public class EBook : Book
    {
        public EBook()
        {
            // Xác định đây là sách điện tử
            BookType = "EBook";
        }

        public override string GetDetails()
        {
            return $"[EBook] {Title} - {Author}";
        }
    }
}