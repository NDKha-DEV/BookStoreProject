// vị trí: Bookstore.Web/Controllers/CartController.cs
using Bookstore.Web.Models;
using Bookstore.Web.Services;
using Microsoft.AspNetCore.Mvc;

namespace Bookstore.Web.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class CartController : ControllerBase
    {
        private readonly ICartManager _cart;

        public CartController(ICartManager cart)
        {
            _cart = cart;
        }

        [HttpGet]
        public ActionResult<CartDto> GetCart()
        {
            return Ok(_cart.GetCart());
        }

        public class AddRequest { public int ProductId { get; set; } public int Quantity { get; set; } = 1; public int? VariantId { get; set; } }
        [HttpPost("add")]
        public ActionResult<CartDto> Add([FromBody] AddRequest req)
        {
            try
            {
                var cart = _cart.AddToCart(req.ProductId, req.Quantity, req.VariantId);
                return Ok(cart);
            }
            catch (CartException ex) when (ex.Code == "OutOfStock")
            {
                return BadRequest(new { error = "Sản phẩm đã hết hàng" });
            }
            catch (CartException ex) when (ex.Code == "NotEnoughStock")
            {
                return BadRequest(new { error = "Số lượng trong giỏ vượt quá tồn kho" });
            }
        }

        public class UpdateRequest { public int ProductId { get; set; } public int Quantity { get; set; } public int? VariantId { get; set; } }
        [HttpPost("update")]
        public ActionResult<CartDto> Update([FromBody] UpdateRequest req)
        {
            if (req.Quantity <= 0) return BadRequest(new { error = "Số lượng phải lớn hơn 0" });
            var cart = _cart.UpdateQuantity(req.ProductId, req.Quantity, req.VariantId);
            return Ok(cart);
        }

        public class RemoveRequest { public int ProductId { get; set; } public int? VariantId { get; set; } }
        [HttpPost("remove")]
        public ActionResult<CartDto> Remove([FromBody] RemoveRequest req)
        {
            var cart = _cart.RemoveItem(req.ProductId, req.VariantId);
            return Ok(cart);
        }
    }
}
