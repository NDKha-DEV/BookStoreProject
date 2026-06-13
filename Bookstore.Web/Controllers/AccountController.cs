// Vị trí: Bookstore.Web/Controllers/AccountController.cs
using Microsoft.AspNetCore.Mvc;
using Bookstore.Web.Modules.NV1_Account;
using Bookstore.Core.Models;

namespace Bookstore.Web.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AccountController : ControllerBase
    {
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

        [HttpPut("profile/update-password")]
        public IActionResult UpdateProfile([FromQuery] string newPassword)
        {
            bool isSuccess = AuthService.Instance.UpdateProfile(newPassword);
            if (isSuccess) return Ok(new { Message = "Cập nhật mật khẩu cá nhân thành công!" });
            return Unauthorized(new { Message = "Bạn chưa đăng nhập hệ thống!" });
        }

        // ==========================================
        // 🔐 HÀM BẢO MẬT ADMIN (Sử dụng Proxy kiểm quyền)
        // ==========================================

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

        [HttpDelete("admin/delete-book/{id}")]
        public IActionResult DeleteBookSecurely(int id)
        {
            bool isAllowed = _accountProxy.ExecuteAdminAction(() =>
            {
                Console.WriteLine($"[CORE ACTION] Hệ thống tiến hành xóa cuốn sách ID {id} khỏi danh mục.");
            });

            if (isAllowed) return Ok(new { Message = $"Hành động thành công: Đã xóa sách ID {id}." });
            return Forbid("Bạn không có quyền truy cập tính năng này! (Yêu cầu quyền Admin)");
        }
    }
}