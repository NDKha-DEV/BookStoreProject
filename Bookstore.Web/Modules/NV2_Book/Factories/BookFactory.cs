using Bookstore.Core.Models;

namespace Bookstore.Web.Modules.NV2_Book.Factories
{
    public class BookFactory
    {
        public static Book CreateBook(string bookType)
        {
            switch (bookType.ToLower())
            {
                case "paper":
                    return new PaperBook();

                case "ebook":
                    return new EBook();

                case "audio":
                    return new AudioBook();

                default:
                    throw new ArgumentException("Loại sách không hợp lệ.");
            }
        }
    }
}