using System;
using System.Linq;
using Bookstore.Core.Interfaces;
using Bookstore.Core.Models;
using Bookstore.Core.Models.NV3_Cart;

namespace Bookstore.Web.Modules.NV3_Cart.Services
{
    public class CartService
    {
        public Cart GetCartByUserId(int userId)
        {
            if (!MockDataStore.UserCarts.ContainsKey(userId))
            {
                MockDataStore.UserCarts[userId] = new Cart();
            }
            return MockDataStore.UserCarts[userId];
        }

        public void AddToCart(int userId, int bookId, int quantity)
        {
            if (quantity <= 0) quantity = 1;

            var book = MockDataStore.Books.FirstOrDefault(b => b.Id == bookId);
            if (book == null) throw new Exception("ProductNotFound");
            if (book.StockQuantity == 0) throw new Exception("OutOfStock");

            var cart = GetCartByUserId(userId);
            var item = cart.Items.FirstOrDefault(i => i.Product.Id == bookId);

            if (item == null)
            {
                if (quantity > book.StockQuantity) throw new Exception("NotEnoughStock");
                cart.Items.Add(new CartItem { Product = book, Quantity = quantity });
            }
            else
            {
                if (item.Quantity + quantity > book.StockQuantity) throw new Exception("NotEnoughStock");
                item.Quantity += quantity;
            }
        }

        public void UpdateQuantity(int userId, int bookId, int quantity)
        {
            var cart = GetCartByUserId(userId);
            var item = cart.Items.FirstOrDefault(i => i.Product.Id == bookId);
            if (item == null) return;

            if (quantity <= 0)
            {
                cart.Items.Remove(item);
                return;
            }

            var book = MockDataStore.Books.FirstOrDefault(b => b.Id == bookId);
            if (book != null && quantity > book.StockQuantity) item.Quantity = book.StockQuantity;
            else item.Quantity = quantity;
        }

        public void RemoveFromCart(int userId, int bookId)
        {
            var cart = GetCartByUserId(userId);
            cart.Items.RemoveAll(i => i.Product.Id == bookId);
        }

        // 🔥 ĐÂY CHÍNH LÀ NƠI ÁP DỤNG DECORATOR PATTERN KHI GỌI TÍNH TỔNG TIỀN
        public decimal GetFinalTotal(int userId, bool applyVat, bool applyGiftWrapping)
        {
            ICart cart = GetCartByUserId(userId); // Component gốc

            if (applyVat)
            {
                cart = new VatTaxDecorator(cart); // Bọc tầng tính thuế
            }

            if (applyGiftWrapping)
            {
                cart = new GiftWrappingDecorator(cart); // Bọc tầng gói quà
            }

            return cart.CalculateTotal(); // Thực thi tính toán qua các lớp bọc
        }
    }
}