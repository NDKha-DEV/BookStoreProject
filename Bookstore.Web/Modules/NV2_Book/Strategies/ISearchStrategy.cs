// Vị trí: Bookstore.Web/Modules/NV2_Book/Strategies/ISearchStrategy.cs
using Bookstore.Core.Models;

namespace Bookstore.Web.Modules.NV2_Book.Strategies
{
    public interface ISearchStrategy
    {
        List<Book> Filter(List<Book> books, string keyword);
    }

    // Chiến lược 1: Tìm kiếm theo Tiêu đề sách
    public class SearchByTitleStrategy : ISearchStrategy
    {
        public List<Book> Filter(List<Book> books, string keyword)
        {
            return books.Where(b => b.Title.Contains(keyword, StringComparison.OrdinalIgnoreCase)).ToList();
        }
    }

    // Chiến lược 2: Tìm kiếm theo Tác giả
    public class SearchByAuthorStrategy : ISearchStrategy
    {
        public List<Book> Filter(List<Book> books, string keyword)
        {
            return books.Where(b => b.Author.Contains(keyword, StringComparison.OrdinalIgnoreCase)).ToList();
        }
    }
}