using System;
using System.Linq;
using Bookstore.Core.Models.NV1_Account;

namespace Bookstore.Web.Modules.NV1_Account.Services
{
    public class AccountProxy
    {
        private readonly AuthService _authService = AuthService.Instance;

        /// <summary>
        /// Bộ lọc bảo mật trung gian: Chỉ thực thi hành động nếu Người dùng có Role nằm trong danh sách cho phép.
        /// </summary>
        /// <param name="allowedRoles">Mảng các Role được phép chạy tính năng này (ví dụ: "ADMIN", "STAFF")</param>
        /// <param name="secureAction">Hành động nghiệp vụ thực tế cần chạy</param>
        public bool ExecuteSecureAction(string[] allowedRoles, Action secureAction)
        {
            var currentUser = _authService.CurrentLoggedInUser;

            // 1. Kiểm tra đăng nhập
            if (currentUser == null)
            {
                Console.WriteLine("[PROXY DENIED] Từ chối truy cập: Chưa đăng nhập hệ thống!");
                return false;
            }

            // 2. Kiểm tra Role có nằm trong danh sách được phép không
            string userRoleUpper = currentUser.Role.ToUpper();
            bool hasPermission = allowedRoles.Select(r => r.ToUpper()).Contains(userRoleUpper);

            if (!hasPermission)
            {
                Console.WriteLine($"[PROXY DENIED] Từ chối: Tài khoản '{currentUser.Username}' (Vai trò: {currentUser.Role}) không có quyền thực hiện hành động này!");
                return false;
            }

            // 3. Hợp lệ -> Cho phép thực thi
            Console.WriteLine($"[PROXY ALLOWED] Xác thực thành công: {currentUser.Role} '{currentUser.Username}' đang thực thi tác vụ.");
            secureAction.Invoke();
            return true;
        }
    }
}