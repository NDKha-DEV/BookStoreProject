// Vị trí: Bookstore.Web/Controllers/PaymentController.cs
using Microsoft.AspNetCore.Mvc;
using Bookstore.Core.Interfaces;
using Bookstore.Core.Models;
using Bookstore.Web.Modules.NV5_Payment;

namespace Bookstore.Web.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class PaymentController : ControllerBase
    {
        private readonly IOrderService _orderService;

        public PaymentController(IOrderService orderService)
        {
            _orderService = orderService;
        }

        [HttpPost("execute-online-payment")]
        public IActionResult ExecutePayment([FromQuery] int orderId, [FromQuery] string gateway = "momo")
        {
            // 1. Tìm đơn hàng xem có đúng đang chờ trả tiền không
            var order = MockDataStore.Orders.FirstOrDefault(o => o.Id == orderId);
            if (order == null) return NotFound(new { Message = "Không thấy đơn hàng!" });
            
            if (order.PaymentStatus == "Paid") return BadRequest(new { Message = "Đơn hàng này đã được thanh toán rồi!" });

            // 2. Khởi tạo đúng đối tượng Adapter theo cấu trúc giao tiếp chung IPaymentProcessor
            IPaymentProcessor processor;
            if (gateway.ToLower() == "vnpay")
            {
                processor = new VnpayAdapter();
            }
            else
            {
                processor = new MomoAdapter();
            }

            // 3. Thực thi thanh toán (Cách 2: Truyền cả orderId và số tiền)
            bool isSuccess = processor.ProcessPayment(order.Id, order.TotalAmount);

            if (isSuccess)
            {
                order.PaymentStatus = "Paid";
                
                // Nếu thanh toán thành công, Adapter ra lệnh cho NV4 cập nhật trạng thái đơn sang Đã thanh toán & Chờ duyệt
                _orderService.ChangeOrderStatus(order.Id, "proceed");

                return Ok(new {
                    Message = $"Giao dịch qua {gateway.ToUpper()} xử lý thành công!",
                    OrderId = order.Id,
                    NewStatus = order.GetFullStatus()
                });
            }

            return BadRequest(new { Message = "Giao dịch thanh toán thất bại từ phía cổng đối tác!" });
        }
    }
}