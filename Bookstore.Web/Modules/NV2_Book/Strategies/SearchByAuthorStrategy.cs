using Bookstore.Core.Interfaces;
using Bookstore.Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Bookstore.Web.Modules.NV2_Book.Strategies
{
    public class SearchByAuthorStrategy : ISearchStrategy
    {
        public List<Book> Search(List<Book> books, string keyword)
        {
            if (string.IsNullOrEmpty(keyword)) return books;
            return books.Where(b => b.Author.Contains(keyword, StringComparison.OrdinalIgnoreCase)).ToList();
        }
    }
}