// Vị trí: Bookstore.Web/Modules/NV1_Account/Services/AccountService.cs
using System.Collections.Generic;
using Bookstore.Core.Interfaces;
using Bookstore.Core.Models.NV1_Account;

namespace Bookstore.Web.Modules.NV1_Account.Services
{
    public class AccountService : IAccountService
    {
        private readonly IUserRepository _userRepository;
        private readonly ICartRepository _cartRepository;

        // ✨ Nhận cổng giao tiếp dữ liệu thông qua DI
        public AccountService(IUserRepository userRepository, ICartRepository cartRepository)
        {
            _userRepository = userRepository;
            _cartRepository = cartRepository;
        }

        public List<User> GetAllAccounts() => _userRepository.GetAll();

        public void CreateAccountByAdmin(User newUser)
        {
            newUser.Id = _userRepository.GetAll().Count + 1;
            _userRepository.Add(newUser);
            
            // Khởi tạo giỏ hàng rỗng cho User mới qua CartRepository
            _cartRepository.GetByUserId(newUser.Id); 
        }

        public void UpdateAccountByAdmin(int id, string role, int points)
        {
            var user = _userRepository.GetById(id);
            if (user != null)
            {
                user.Role = role;
                user.LoyaltyPoints = points;
                _userRepository.Update(user);
            }
        }

        public void DeleteAccountByAdmin(int id)
        {
            _userRepository.Delete(id);
        }
    }
}