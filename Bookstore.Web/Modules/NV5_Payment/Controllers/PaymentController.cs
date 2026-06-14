// Vị trí: Bookstore.Web/Modules/NV5_Payment/Controllers/PaymentController.cs
using Microsoft.AspNetCore.Mvc;
using System;
using Bookstore.Web.Modules.NV5_Payment.Services;
using Bookstore.Core.Interfaces;

namespace Bookstore.Web.Modules.NV5_Payment.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class PaymentController : ControllerBase
    {
        private readonly PaymentService _paymentService;
        private readonly IOrderRepository _orderRepository;

        // DI Container sẽ tự động điền các tham số dịch vụ này khi nhận Request
        public PaymentController(PaymentService paymentService, IOrderRepository orderRepository)
        {
            _paymentService = paymentService;
            _orderRepository = orderRepository;
        }

        [HttpPost("execute-payment")]
        public IActionResult ExecutePayment([FromQuery] int orderId, [FromQuery] string paymentMethod)
        {
            try
            {
                bool result = _paymentService.ProcessOrderPayment(orderId, paymentMethod);
                var order = _orderRepository.GetById(orderId);
                
                if (order == null)
                {
                    return NotFound(new { Message = $"Không tìm thấy thông tin đơn hàng ID {orderId} trong hệ thống!" });
                }
                
                if (result)
                {
                    return Ok(new {
                        Message = $"Xử lý thanh toán qua cổng {paymentMethod} thành công!",
                        OrderId = orderId,
                        PaymentMethod = order.PaymentMethod,
                        PaymentStatus = order.PaymentStatus,
                        CurrentOrderState = order.CurrentState.GetStatusName()
                    });
                }
                
                return BadRequest(new { Message = "Thanh toán thất bại hoặc giao dịch bị hủy bỏ." });
            }
            catch (Exception ex)
            {
                return BadRequest(new { Error = ex.Message });
            }
        }
    }
}