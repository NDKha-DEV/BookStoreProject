// 2. NV2: IBookService.cs - Quản lý sản phẩm sách
using Bookstore.Core.Models;
namespace Bookstore.Core.Interfaces {
    public interface IBookService {
        List<Book> GetAllBooks();
        Book GetBookById(int id);
        void UpdateStock(int bookId, int quantity); // Dùng để trừ kho khi mua xong
    }
}