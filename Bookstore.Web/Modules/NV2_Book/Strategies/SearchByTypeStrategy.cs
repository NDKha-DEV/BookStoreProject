using Bookstore.Core.Interfaces;
using Bookstore.Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Bookstore.Web.Modules.NV2_Book.Strategies
{
    public class SearchByTypeStrategy : ISearchStrategy
    {
        public List<Book> Search(List<Book> books, string keyword)
        {
            if (string.IsNullOrEmpty(keyword)) return books;
            // Lọc theo tên loại sách (PaperBook, EBook, AudioBook)
            return books.Where(b => b.GetType().Name.Contains(keyword, StringComparison.OrdinalIgnoreCase)).ToList();
        }
    }
}