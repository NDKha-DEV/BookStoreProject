// Vị trí: Bookstore.Web/Modules/NV5_Payment/Controllers/PaymentController.cs
using Microsoft.AspNetCore.Mvc;
using System;
using Bookstore.Web.Modules.NV5_Payment.Services;
using Bookstore.Core.Models;
using System.Linq;

namespace Bookstore.Web.Modules.NV5_Payment.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class PaymentController : ControllerBase
    {
        private readonly PaymentService _paymentService = new PaymentService();

        [HttpPost("execute-payment")]
        public IActionResult ExecutePayment([FromQuery] int orderId, [FromQuery] string paymentMethod)
        {
            try
            {
                // 🔥 NV5 LÀM CHỦ LUỒNG CHỌN PHƯƠNG THỨC VÀ XỬ LÝ TIỀN TẠI ĐÂY
                bool result = _paymentService.ProcessOrderPayment(orderId, paymentMethod);

                var order = MockDataStore.Orders.FirstOrDefault(o => o.Id == orderId);
                
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
                        CurrentOrderState = order.CurrentState.GetStatusName() // Sẽ tự động nhảy lên Chờ duyệt/Pending nhờ hàm .Proceed()
                    });
                }
                else
                {
                    return BadRequest(new { Message = "Thanh toán thất bại hoặc giao dịch bị hủy bỏ." });
                }
            }
            catch (Exception ex)
            {
                return BadRequest(new { Error = ex.Message });
            }
        }
    }
}