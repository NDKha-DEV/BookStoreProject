// vị trí: Bookstore.Core/Models/NV2_Book/ISearchStrategy.cs
using System.Collections.Generic;

namespace Bookstore.Core.Models.NV2_Book
{
    public interface ISearchStrategy
    {
        List<Book> Search(List<Book> books, string keyword);
    }
}