using Bookstore.Core.Models;

namespace Bookstore.Core.Models
{
    public class AudioBook : Book
    {
        public AudioBook()
        {
            // Xác định đây là sách nói
            BookType = "Audio";
        }

        public override string GetDetails()
        {
            return $"[Audio Book] {Title} - {Author}";
        }
    }
}