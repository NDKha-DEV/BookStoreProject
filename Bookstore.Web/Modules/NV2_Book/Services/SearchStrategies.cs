// vị trí: Bookstore.Web/Modules/NV2_Book/Services/SearchStrategies.cs
using System.Collections.Generic;
using System.Linq;
using Bookstore.Core.Models.NV2_Book;

namespace Bookstore.Web.Modules.NV2_Book.Services
{
    public class SearchByTitleStrategy : ISearchStrategy
    {
        public List<Book> Search(List<Book> books, string keyword)
        {
            if (string.IsNullOrEmpty(keyword)) return books;
            return books.Where(b => b.Title.ToLower().Contains(keyword.ToLower())).ToList();
        }
    }

    public class SearchByAuthorStrategy : ISearchStrategy
    {
        public List<Book> Search(List<Book> books, string keyword)
        {
            if (string.IsNullOrEmpty(keyword)) return books;
            return books.Where(b => b.Author.ToLower().Contains(keyword.ToLower())).ToList();
        }
    }

    public class SearchByTypeStrategy : ISearchStrategy
    {
        public List<Book> Search(List<Book> books, string keyword)
        {
            if (string.IsNullOrEmpty(keyword)) return books;
            return books.Where(b => b.BookType.ToLower().Contains(keyword.ToLower())).ToList();
        }
    }

    public class SearchService
    {
        private readonly ISearchStrategy _strategy;

        public SearchService(ISearchStrategy strategy)
        {
            _strategy = strategy;
        }

        public List<Book> Search(List<Book> books, string keyword)
        {
            return _strategy.Search(books, keyword);
        }
    }
}