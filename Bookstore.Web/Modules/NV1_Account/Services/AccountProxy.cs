// vị trí: Bookstore.Web/Modules/NV1_Account/Services/AccountProxy.cs
using System;
using Bookstore.Core.Models.NV1_Account;

namespace Bookstore.Web.Modules.NV1_Account.Services
{
    public class AccountProxy
    {
        private readonly AuthService _authService = AuthService.Instance;

        /// <summary>
        /// Bảo vệ các tính năng nhạy cảm (như duyệt đơn hàng của NV4). Chỉ cho phép Admin đi qua.
        /// </summary>
        public bool ExecuteAdminAction(Action adminAction)
        {
            var currentUser = _authService.CurrentLoggedInUser;

            if (currentUser == null)
            {
                Console.WriteLine("[ PROXY DENIED] Từ chối truy cập: Chưa có người dùng nào đăng nhập hệ thống!");
                return false;
            }

            if (currentUser.Role.ToUpper() != "ADMIN")
            {
                Console.WriteLine($"[ PROXY DENIED] Từ chối truy cập: Tài khoản '{currentUser.Username}' (Vai trò: {currentUser.Role}) không có quyền Admin!");
                return false;
            }

            // Nếu vượt qua vòng kiểm tra bảo mật, cho phép thực thi hành động thực tế
            Console.WriteLine($"[ PROXY ALLOWED] Xác thực thành công: Admin '{currentUser.Username}' được phép thực hiện hành động.");
            adminAction.Invoke();
            return true;
        }
    }
}