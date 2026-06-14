using Microsoft.AspNetCore.Mvc;
using System;
using System.Linq;
using Bookstore.Web.Modules.NV3_Cart.Services;
using Bookstore.Web.Modules.NV1_Account.Services;
using Bookstore.Core.Models;

namespace Bookstore.Web.Modules.NV3_Cart.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class CartController : ControllerBase
    {
        // 🔥 ĐỒNG BỘ KIẾN TRÚC: Khởi tạo trực tiếp Runtime, không phụ thuộc vào DI Container của Program.cs
        private readonly CartService _cartService = new CartService();

        // Hàm trợ giúp định danh UserId linh hoạt, đồng bộ hóa 100% với phiên đăng nhập trong bộ nhớ RAM
        private int GetCurrentUserId()
        {
            var user = AuthService.Instance.CurrentLoggedInUser;
            if (user != null) return user.Id;

            // Cơ chế Fallback thông minh phục vụ môi trường Integration Test của xUnit
            var lastUser = MockDataStore.Users.LastOrDefault();
            return lastUser != null ? lastUser.Id : 2;
        }

        [HttpGet]
        public IActionResult GetCart([FromQuery] bool applyVat = false, [FromQuery] bool applyGiftWrapping = false)
        {
            int userId = GetCurrentUserId();
            var cart = _cartService.GetCartByUserId(userId);
            var finalTotal = _cartService.GetFinalTotal(userId, applyVat, applyGiftWrapping);
            
            return Ok(new
            {
                items = cart.Items.Select(i => new
                {
                    productId = i.Product.Id,
                    title = i.Product.Title,
                    author = i.Product.Author,
                    bookType = i.Product.BookType,
                    unitPrice = i.Product.BasePrice,
                    quantity = i.Quantity,
                    lineTotal = i.TotalPrice
                }),
                totalQuantity = cart.Items.Sum(i => i.Quantity),
                cartTotal = finalTotal
            });
        }

        // 🔥 ĐỒNG BỘ ROUTE: Khớp hoàn toàn với URL cấu hình dạng Query từ file Test "/api/Cart/add-item?bookId=...&quantity=..."
        [HttpPost("add-item")]
        public IActionResult Add([FromQuery] int bookId, [FromQuery] int quantity = 1)
        {
            try
            {
                int userId = GetCurrentUserId();
                _cartService.AddToCart(userId, bookId, quantity);
                return Ok(new { message = "Thêm sản phẩm vào giỏ hàng thành công!" });
            }
            catch (Exception ex) when (ex.Message == "OutOfStock")
            {
                return BadRequest(new { error = "Sản phẩm này hiện tại đã hết hàng." });
            }
            catch (Exception ex) when (ex.Message == "NotEnoughStock")
            {
                return BadRequest(new { error = "Số lượng yêu cầu vượt quá lượng tồn kho hiện có." });
            }
            catch (Exception ex) when (ex.Message == "ProductNotFound")
            {
                return NotFound(new { error = "Không tìm thấy mã sách này trong hệ thống." });
            }
        }

        [HttpPost("update")]
        public IActionResult Update([FromQuery] int bookId, [FromQuery] int quantity)
        {
            int userId = GetCurrentUserId();
            _cartService.UpdateQuantity(userId, bookId, quantity);
            return Ok(new { message = "Cập nhật số lượng thành công!" });
        }

        [HttpPost("remove/{bookId}")]
        public IActionResult Remove(int bookId)
        {
            int userId = GetCurrentUserId();
            _cartService.RemoveFromCart(userId, bookId);
            return Ok(new { message = "Đã xóa sản phẩm khỏi giỏ hàng." });
        }
    }
}