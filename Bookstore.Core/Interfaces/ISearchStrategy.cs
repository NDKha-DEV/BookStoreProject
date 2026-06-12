using Bookstore.Core.Models;

namespace Bookstore.Core.Interfaces
{
    public interface ISearchStrategy
    {
        List<Book> Search(List<Book> books, string keyword);
    }
}