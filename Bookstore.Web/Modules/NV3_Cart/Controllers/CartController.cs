using Microsoft.AspNetCore.Mvc;
using System;
using System.Linq;
using Bookstore.Web.Modules.NV3_Cart.Services;

namespace Bookstore.Web.Modules.NV3_Cart.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class CartController : ControllerBase
    {
        private readonly CartService _cartService;
        private const int CurrentUserId = 2; 

        public CartController(CartService cartService)
        {
            _cartService = cartService;
        }

        [HttpGet]
        public IActionResult GetCart([FromQuery] bool applyVat = false, [FromQuery] bool applyGiftWrapping = false)
        {
            var cart = _cartService.GetCartByUserId(CurrentUserId);
            
            // 🔥 SỬA LỖI: Gọi hàm GetFinalTotal đã tích hợp Decorator Pattern chuẩn chỉ
            var finalTotal = _cartService.GetFinalTotal(CurrentUserId, applyVat, applyGiftWrapping);
            
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

        public class CartRequest { public int ProductId { get; set; } public int Quantity { get; set; } = 1; }

        [HttpPost("add")]
        public IActionResult Add([FromBody] CartRequest req)
        {
            try
            {
                _cartService.AddToCart(CurrentUserId, req.ProductId, req.Quantity);
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
        public IActionResult Update([FromBody] CartRequest req)
        {
            _cartService.UpdateQuantity(CurrentUserId, req.ProductId, req.Quantity);
            return Ok(new { message = "Cập nhật số lượng thành công!" });
        }

        [HttpPost("remove/{bookId}")]
        public IActionResult Remove(int bookId)
        {
            _cartService.RemoveFromCart(CurrentUserId, bookId);
            return Ok(new { message = "Đã xóa sản phẩm khỏi giỏ hàng." });
        }
    }
}