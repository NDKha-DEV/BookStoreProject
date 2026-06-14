// vị trí: Bookstore.Core/Models/NV1_Account/IAuthService.cs
namespace Bookstore.Core.Models.NV1_Account
{
    public interface IAuthService
    {
        bool Register(string username, string password);
        bool Login(string username, string password);
        void Logout();
        bool UpdateProfile(string fullName, string address, string? newPassword);
        string GetCurrentUserRole(); 
    }
}