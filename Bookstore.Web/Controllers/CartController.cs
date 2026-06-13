// Vị trí: Bookstore.Web/Controllers/CartController.cs
using Microsoft.AspNetCore.Mvc;
using Bookstore.Core.Models;
using Bookstore.Core.Interfaces;
using Bookstore.Web.Modules.NV3_Cart;
using Bookstore.Web.Modules.NV3_Cart.Strategies;
using Bookstore.Web.Modules.NV3_Cart.Decorators;
using Bookstore.Web.Modules.NV1_Account;

namespace Bookstore.Web.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class CartController : ControllerBase
    {
        private readonly CartService _cartService = new CartService();

        private int GetCurrentUserId()
        {
            // Tự động nhận diện ID khách hàng đang đăng nhập, mặc định là ID = 3 (Bạn Long mẫu)
            var user = AuthService.Instance.CurrentLoggedInUser;
            return user != null ? user.Id : 3;
        }

        [HttpGet]
        public IActionResult GetMyCart()
        {
            int userId = GetCurrentUserId();
            var cartItems = _cartService.GetCart(userId);
            return Ok(new { UserId = userId, TotalItems = cartItems.Count, Items = cartItems });
        }

        [HttpPost("add-item")]
        public IActionResult AddToCart([FromQuery] int bookId, [FromQuery] int quantity = 1)
        {
            try
            {
                int userId = GetCurrentUserId();
                _cartService.AddToCart(userId, bookId, quantity);
                return Ok(new { Message = "Đã thêm sản phẩm vào giỏ hàng thành công!" });
            }
            catch (Exception ex)
            {
                return BadRequest(new { Error = ex.Message });
            }
        }

        [HttpPut("update-quantity")]
        public IActionResult UpdateQuantity([FromQuery] int bookId, [FromQuery] int newQuantity)
        {
            int userId = GetCurrentUserId();
            _cartService.UpdateQuantity(userId, bookId, newQuantity);
            return Ok(new { Message = "Cập nhật số lượng thành công!" });
        }

        [HttpDelete("remove-item/{bookId}")]
        public IActionResult RemoveItem(int bookId)
        {
            int userId = GetCurrentUserId();
            _cartService.RemoveFromCart(userId, bookId);
            return Ok(new { Message = "Đã xóa sản phẩm khỏi giỏ hàng." });
        }

        [HttpGet("preview-checkout")]
        public IActionResult PreviewCheckout([FromQuery] decimal distanceInKm, [FromQuery] bool usePremiumPackaging = false)
        {
            int userId = GetCurrentUserId();

            var cartItems = _cartService.GetCart(userId);

            if (cartItems == null || cartItems.Count == 0) 
            {
                return BadRequest(new { Message = "Giỏ hàng trống! Hãy thêm sách trước khi xem hóa đơn." });
            }
            
            // 🌟 ĐÃ ĐỘNG: Lấy tổng tiền hàng thật đang có trong giỏ thay vì gán cứng 250k
            decimal baseItemsTotal = _cartService.GetCartItemsTotal(userId);
            if (baseItemsTotal == 0) return BadRequest(new { Message = "Giỏ hàng trống! Hãy thêm sách trước khi xem hóa đơn." });

            Cart baseCart = new Cart { DeliveryDistanceInKm = distanceInKm };

            // Sử dụng Strategy Pattern tính tiền ship
            IShippingStrategy shippingStrategy = distanceInKm < 5 
                ? new StandardShippingStrategy() 
                : new ExpressShippingStrategy();
            decimal shippingFee = shippingStrategy.CalculateShippingFee(distanceInKm);

            // Sử dụng Decorator Pattern nếu khách tích chọn gói quà cao cấp
            decimal finalTotal = baseItemsTotal + shippingFee;
            bool decoratorActivated = false;

            if (usePremiumPackaging)
            {
                var decoratedCart = new PremiumPackagingDecorator(baseCart);
                finalTotal = decoratedCart.GetTotalCostWithService(baseItemsTotal, shippingFee);
                decoratorActivated = true;
            }

            return Ok(new {
                CartBaseAmount = baseItemsTotal,
                Distance = distanceInKm + " km",
                ShippingFee = shippingFee,
                IsPremiumPackagingApplied = decoratorActivated,
                FinalTotalPayable = finalTotal
            });
        }
    }
}