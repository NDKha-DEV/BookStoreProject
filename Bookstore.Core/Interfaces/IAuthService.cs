// 1. NV1: IAuthService.cs - Quản lý tài khoản
namespace Bookstore.Core.Interfaces {
    public interface IAuthService {
        bool Register(string username, string password);
        bool Login(string username, string password);
        string GetCurrentUserRole(); // Trả về "Admin" hoặc "Customer"
    }
}