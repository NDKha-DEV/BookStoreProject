// Vị trí: Bookstore.Web/Controllers/OrderController.cs
using Bookstore.Core.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Bookstore.Web.Modules.NV3_Cart;
using Bookstore.Web.Modules.NV3_Cart.Strategies;
using Bookstore.Web.Modules.NV3_Cart.Decorators;
using Bookstore.Web.Modules.NV1_Account.Services;
using Bookstore.Core.Models;

namespace Bookstore.Web.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class OrderController : ControllerBase
    {
        private readonly IOrderService _orderService;
        private readonly CartService _cartService = new CartService();

        public OrderController(IOrderService orderService)
        {
            _orderService = orderService;
        }

        [HttpPost("create-order")]
        public IActionResult Create([FromQuery] string paymentMethod, [FromQuery] decimal distanceInKm = 2, [FromQuery] bool usePremiumPackaging = false)
        {
            try
            {
                var user = AuthService.Instance.CurrentLoggedInUser;
                int userId = user != null ? user.Id : 3;

                var cartItems = _cartService.GetCart(userId);
                if (cartItems == null || cartItems.Count == 0) 
                {
                    throw new Exception("Giỏ hàng của bạn đang trống!");
                }

                // 1. Tính toán số tiền động thực tế của giỏ hàng để đưa vào đơn hàng
                decimal itemsTotal = _cartService.GetCartItemsTotal(userId);
                if (itemsTotal == 0) throw new Exception("Giỏ hàng của bạn đang trống!");

                IShippingStrategy shippingStrategy = distanceInKm < 5 ? new StandardShippingStrategy() : new ExpressShippingStrategy();
                decimal shippingFee = shippingStrategy.CalculateShippingFee(distanceInKm);

                decimal finalDynamicTotal = itemsTotal + shippingFee;
                if (usePremiumPackaging)
                {
                    var decoratedCart = new PremiumPackagingDecorator(new Cart());
                    finalDynamicTotal = decoratedCart.GetTotalCostWithService(itemsTotal, shippingFee);
                }

                // 2. Gọi hàm CreateOrder mới đã đồng bộ 2 tham số
                var order = _orderService.CreateOrder(paymentMethod, finalDynamicTotal);

                // 3. Đặt hàng thành công thì tự động làm sạch giỏ hàng của User
                _cartService.ClearCart(userId);

                return Ok(new {
                    Message = "Tạo đơn hàng thành công!",
                    OrderId = order.Id,
                    TotalAmount = order.TotalAmount,
                    Method = order.PaymentMethod,
                    CurrentStateName = order.CurrentState.GetStatusName(),
                    FullSystemStatus = order.GetFullStatus()
                });
            }
            catch (Exception ex)
            {
                return BadRequest(new { Error = ex.Message });
            }
        }

        [HttpPut("{id}/process-next-step")]
        public IActionResult Process(int id, [FromQuery] string action = "proceed")
        {
            try
            {
                _orderService.ChangeOrderStatus(id, action);
                var order = _orderService.GetOrderDetails(id);
                return Ok(new {
                    Message = "Thao tác duyệt trạng thái thành công!",
                    CurrentStateName = order.CurrentState.GetStatusName(),
                    FullSystemStatus = order.GetFullStatus()
                });
            }
            catch (Exception ex)
            {
                return BadRequest(new { Error = ex.Message });
            }
        }
    }
}