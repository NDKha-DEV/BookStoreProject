using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using Bookstore.Web.Modules.NV1_Account.Services;
using Bookstore.Core.Models.NV1_Account;

namespace Bookstore.Web.Modules.NV1_Account.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AccountController : ControllerBase
    {
        public class UpdateProfileDto 
        {
            public string FullName { get; set; } = string.Empty;
            public string Address { get; set; } = string.Empty;
            public string? NewPassword { get; set; } 
        }
        private readonly AccountProxy _accountProxy = new AccountProxy();
        private readonly AccountService _accountService = new AccountService();

        [HttpPost("login")]
        public IActionResult Login([FromQuery] string username, [FromQuery] string password)
        {
            bool isSuccess = AuthService.Instance.Login(username, password);
            if (isSuccess)
            {
                var user = AuthService.Instance.CurrentLoggedInUser;
                if (user == null) return BadRequest(new { Message = "Không thể thiết lập phiên đăng nhập!" });

                return Ok(new { Message = "Đăng nhập thành công!", User = user.Username, Role = user.Role });
            }
            return BadRequest(new { Message = "Sai tài khoản hoặc mật khẩu!" });
        }

        [HttpPost("logout")]
        public IActionResult Logout()
        {
            AuthService.Instance.Logout();
            return Ok(new { Message = "Đã đăng xuất hệ thống." });
        }

        [HttpPost("register")]
        public IActionResult Register([FromQuery] string username, [FromQuery] string password)
        {
            bool isSuccess = AuthService.Instance.Register(username, password);
            if (isSuccess) return Ok(new { Message = "Đăng ký tài khoản khách hàng thành công!" });
            return BadRequest(new { Message = "Tên tài khoản đã tồn tại!" });
        }

        [HttpPut("profile/update")]
        public IActionResult UpdateProfile([FromBody] UpdateProfileDto request)
        {
            bool isSuccess = AuthService.Instance.UpdateProfile(request.FullName, request.Address, request.NewPassword);
            if (isSuccess) return Ok(new { Message = "Cập nhật thông tin cá nhân (Tên, Địa chỉ, Mật khẩu) thành công!" });
            return Unauthorized(new { Message = "Bạn chưa đăng nhập hệ thống!" });
        }

        [HttpGet("admin/all-accounts")]
        public IActionResult GetAllAccounts()
        {
            List<User>? data = null;
            bool isAllowed = _accountProxy.ExecuteAdminAction(() => {
                data = _accountService.GetAllAccounts();
            });

            if (isAllowed) return Ok(data);
            return Forbid("Bạn không có quyền! Yêu cầu tài khoản ADMIN.");
        }

        [HttpPost("admin/create-account")]
        public IActionResult CreateAccountByAdmin([FromBody] User newUser)
        {
            bool isAllowed = _accountProxy.ExecuteAdminAction(() => {
                _accountService.CreateAccountByAdmin(newUser);
            });

            if (isAllowed) return Ok(new { Message = "Admin đã thêm tài khoản mới thành công!" });
            return Forbid("Bạn không có quyền! Yêu cầu tài khoản ADMIN.");
        }

        [HttpPut("admin/update-account/{id}")]
        public IActionResult UpdateAccountByAdmin(int id, [FromQuery] string role, [FromQuery] int points = 0)
        {
            bool isAllowed = _accountProxy.ExecuteAdminAction(() => {
                _accountService.UpdateAccountByAdmin(id, role, points);
            });

            if (isAllowed) 
                return Ok(new { Message = $"Cập nhật quyền thành công cho tài khoản ID: {id} sang [{role}]." });
                
            return Forbid("Bạn không có quyền! Yêu cầu tài khoản ADMIN.");
        }

        [HttpDelete("admin/delete-account/{id}")]
        public IActionResult DeleteAccountByAdmin(int id)
        {
            bool isAllowed = _accountProxy.ExecuteAdminAction(() => {
                _accountService.DeleteAccountByAdmin(id);
            });

            if (isAllowed) 
                return Ok(new { Message = $"Đã xóa vĩnh viễn tài khoản ID: {id} khỏi hệ thống." });
                
            return Forbid("Bạn không có quyền! Yêu cầu tài khoản ADMIN.");
        }

        [HttpDelete("admin/delete-book/{id}")]
        public IActionResult DeleteBookSecurely(int id)
        {
            bool isAllowed = _accountProxy.ExecuteAdminAction(() =>
            {
                System.Console.WriteLine($"[CORE ACTION] Hệ thống tiến hành xóa cuốn sách ID {id} khỏi danh mục.");
            });

            if (isAllowed) return Ok(new { Message = $"Hành động thành công: Đã xóa sách ID {id}." });
            return Forbid("Bạn không có quyền truy cập tính năng này! (Yêu cầu quyền Admin)");
        }
    }
}