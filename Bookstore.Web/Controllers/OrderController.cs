// Trong file: Bookstore.Web/Controllers/OrderController.cs
using Bookstore.Core.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace Bookstore.Web.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class OrderController : ControllerBase
    {
        private readonly IOrderService _orderService;

        public OrderController(IOrderService orderService)
        {
            _orderService = orderService;
        }

        // 1. Tạo đơn hàng thử nghiệm
        [HttpPost("create")]
        public IActionResult Create()
        {
            var order = _orderService.CreateOrderFromCart();
            return Ok(new { 
                Message = "Tạo đơn thành công!", 
                OrderId = order.Id, 
                Status = order.CurrentState.GetStatusName(),
                Total = order.TotalAmount
            });
        }

        // 2. Thay đổi trạng thái đơn hàng (Dành cho Admin duyệt)
        // Truyền action = "proceed" để duyệt tiến lên, hoặc "cancel" để hủy
        [HttpPut("{id}/change-status")]
        public IActionResult UpdateStatus(int id, [FromQuery] string action)
        {
            try
            {
                _orderService.ChangeOrderStatus(id, action);
                var order = _orderService.GetOrderDetails(id);
                return Ok(new { 
                    Message = "Cập nhật trạng thái thành công!", 
                    NewStatus = order.CurrentState.GetStatusName() 
                });
            }
            catch (Exception ex)
            {
                return BadRequest(new { Error = ex.Message });
            }
        }
    }
}