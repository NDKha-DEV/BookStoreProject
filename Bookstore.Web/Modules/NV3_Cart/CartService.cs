// Vị trí: Bookstore.Web/Modules/NV3_Cart/CartService.cs
using Bookstore.Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Bookstore.Web.Modules.NV3_Cart
{
    public class CartService
    {
        // Lấy thông tin giỏ hàng hiện tại của User (R trong CRUD)
        public List<CartItem> GetCart(int userId)
        {
            if (!MockDataStore.UserCarts.ContainsKey(userId))
                MockDataStore.UserCarts[userId] = new List<CartItem>();
            return MockDataStore.UserCarts[userId];
        }

        // Thêm sản phẩm vào giỏ hàng (C trong CRUD)
        public void AddToCart(int userId, int bookId, int quantity)
        {
            var book = MockDataStore.Books.FirstOrDefault(b => b.Id == bookId);
            if (book == null) throw new Exception("Sách không tồn tại!");
            if (book.StockQuantity < quantity) throw new Exception("Số lượng trong kho không đủ!");

            var cart = GetCart(userId);
            // Cập nhật: Tìm theo i.Product.Id thay vì i.BookId
            var existingItem = cart.FirstOrDefault(i => i.Product.Id == bookId);

            if (existingItem != null)
            {
                existingItem.Quantity += quantity;
            }
            else
            {
                // Cập nhật: Khởi tạo theo cấu trúc class CartItem mới của Core
                cart.Add(new CartItem { Product = book, Quantity = quantity });
            }
        }

        // Sửa số lượng sản phẩm trong giỏ (U trong CRUD)
        public void UpdateQuantity(int userId, int bookId, int newQuantity)
        {
            var cart = GetCart(userId);
            // Cập nhật: Tìm theo i.Product.Id
            var item = cart.FirstOrDefault(i => i.Product.Id == bookId);
            if (item != null)
            {
                if (newQuantity <= 0) { RemoveFromCart(userId, bookId); return; }
                item.Quantity = newQuantity;
            }
        }

        // Xóa sản phẩm khỏi giỏ hàng (D trong CRUD)
        public void RemoveFromCart(int userId, int bookId)
        {
            var cart = GetCart(userId);
            // Cập nhật: Tìm theo i.Product.Id
            var item = cart.FirstOrDefault(i => i.Product.Id == bookId);
            if (item != null) cart.Remove(item);
        }

        // Tính tiền hàng gốc dựa trên dữ liệu chuẩn hướng đối tượng
        public decimal GetCartItemsTotal(int userId)
        {
            // Cập nhật: Dùng item.TotalPrice đã được định nghĩa sẵn bên trong CartItem
            return GetCart(userId).Sum(item => item.TotalPrice);
        }

        // Hàm xóa sạch giỏ sau khi đặt hàng thành công
        public void ClearCart(int userId)
        {
            GetCart(userId).Clear();
        }
    }
}