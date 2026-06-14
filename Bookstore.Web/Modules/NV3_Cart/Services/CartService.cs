// Vị trí: Bookstore.Web/Modules/NV3_Cart/Services/CartService.cs
using System;
using System.Linq;
using Bookstore.Core.Interfaces;
using Bookstore.Core.Models;
using Bookstore.Core.Models.NV3_Cart;
using Bookstore.Core.Models.NV4_Order.Interfaces;
using Bookstore.Web.Modules.NV4_Order.Services;

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
        // 🔥 ĐÃ ĐỒNG BỘ: Tính toán trọn gói từ Tiền hàng, Decorator (Thuế/Quà) đến Strategy (Phí vận chuyển)
        public decimal GetFinalTotal(int userId, bool applyVat, bool applyGiftWrapping)
        {
            // 1. Lấy giỏ hàng gốc của người dùng
            var cart = GetCartByUserId(userId); 
            ICart decoratedCart = cart; // Thành phần gốc ban đầu phục vụ Decorator

            // 2. Áp dụng Decorator Pattern để tính toán thuế và dịch vụ gói quà trên tiền hàng
            if (applyVat)
            {
                decoratedCart = new VatTaxDecorator(decoratedCart); 
            }

            if (applyGiftWrapping)
            {
                decoratedCart = new GiftWrappingDecorator(decoratedCart); 
            }

            // Đây là số tiền hàng sau khi đã cộng các dịch vụ gia tăng (nếu có)
            decimal totalProductAndServices = decoratedCart.CalculateTotal(); 

            // 3. Áp dụng Strategy Pattern để tính phí vận chuyển dựa trên thuộc tính Km của Cart
            IShippingStrategy shippingStrategy = cart.DeliveryDistanceInKm < 5 
                ? new StandardShippingStrategy() 
                : new ExpressShippingStrategy();

            decimal shippingFee = shippingStrategy.CalculateShippingFee(cart.DeliveryDistanceInKm);

            // Trả về con số cuối cùng trọn gói bàn giao cho đơn hàng
            return totalProductAndServices + shippingFee;
        }
    }
}