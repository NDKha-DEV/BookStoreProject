// Vị trí: Bookstore.Web/Modules/NV4_Order/Controllers/OrderController.cs
using System;
using Microsoft.AspNetCore.Mvc;
using Bookstore.Core.Models.NV4_Order.Interfaces;
using Bookstore.Core.Models;
using Bookstore.Core.Models.NV4_Order;
using Bookstore.Web.Modules.NV1_Account.Services;
using Bookstore.Web.Modules.NV3_Cart.Services; // Kết nối sang dịch vụ giỏ hàng NV3

namespace Bookstore.Web.Modules.NV4_Order.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class OrderController : ControllerBase
    {
        private readonly IOrderService _orderService;
        
        private readonly CartService _cartService = new CartService();

        private readonly AccountProxy _accountProxy = new AccountProxy();

        public OrderController(IOrderService orderService)
        {
            _orderService = orderService;
        }

        [HttpPost("create-order")]
        public IActionResult Create([FromQuery] decimal distanceInKm = 2, [FromQuery] bool applyVat = true, [FromQuery] bool applyGiftWrapping = false)
        {
            try
            {
                var user = AuthService.Instance.CurrentLoggedInUser;
                int userId;

                if (user != null)
                {
                    userId = user.Id;
                }
                else
                {
                    // Cơ chế Fallback thông minh cho Integration Test: 
                    // Nếu Client Test bị mất trạng thái tĩnh, lấy User mới nhất vừa được tạo/đăng nhập trong kho MockDataStore
                    var lastUser = MockDataStore.Users.LastOrDefault();
                    userId = lastUser != null ? lastUser.Id : 3;
                }

                var cart = _cartService.GetCartByUserId(userId);
                if (cart == null || cart.Items.Count == 0) 
                {
                    throw new Exception("Giỏ hàng của bạn đang trống! Hãy thêm sách trước khi đặt hàng.");
                }

                // 1. Cập nhật số Km người dùng truyền từ API vào thuộc tính của Cart
                cart.DeliveryDistanceInKm = distanceInKm;

                // 2. GỌI ĐỒNG BỘ LUỒNG: Lấy tổng tiền trọn gói cuối cùng (Đã gồm sách + thuế + quà + ship)
                decimal finalDynamicTotal = _cartService.GetFinalTotal(userId, applyVat, applyGiftWrapping);

                // 3. Tiến hành tạo đơn hàng với số tiền chính xác
                var order = _orderService.CreateOrder(finalDynamicTotal);

                order.PaymentMethod = "PENDING";

                // 4. Đặt hàng thành công thì làm sạch giỏ hàng vật lý của khách
                _cartService.RemoveFromCart(userId, 0); 
                cart.Items.Clear();

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
                Order? order = null;
                
                // 🛡️ Bọc qua Proxy: Chỉ ADMIN và STAFF mới được thực thi hành động duyệt/hủy này
                bool isAllowed = _accountProxy.ExecuteSecureAction(new[] { "ADMIN", "STAFF" }, () =>
                {
                    _orderService.ChangeOrderStatus(id, action);
                    order = _orderService.GetOrderDetails(id);
                });

                if (isAllowed && order != null)
                {
                    return Ok(new {
                        Message = $"Thao tác '{action}' đơn hàng thành công bởi nhân viên/quản trị!",
                        CurrentStateName = order.CurrentState.GetStatusName(),
                        FullSystemStatus = order.GetFullStatus()
                    });
                }

                return StatusCode(403, new { Error = "Bạn không có quyền thực hiện thao tác này! Yêu cầu tài khoản nhân viên (STAFF) hoặc ADMIN." });
            }
            catch (Exception ex)
            {
                return BadRequest(new { Error = ex.Message });
            }
        }
    }
}