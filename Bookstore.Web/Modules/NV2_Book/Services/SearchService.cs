using Bookstore.Core.Interfaces;
using Bookstore.Core.Models;

namespace Bookstore.Web.Modules.NV2_Book.Services
{
    public class SearchService
    {
        private readonly ISearchStrategy _searchStrategy;

        public SearchService(ISearchStrategy searchStrategy)
        {
            _searchStrategy = searchStrategy;
        }

        public List<Book> Search(List<Book> books, string keyword)
        {
            return _searchStrategy.Search(books, keyword);
        }
    }
}
