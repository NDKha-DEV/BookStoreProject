// Vị trí: Bookstore.Web/Modules/NV1_Account/Services/AuthService.cs
using System;
using Bookstore.Core.Interfaces;
using Bookstore.Core.Models.NV1_Account;
using Bookstore.Core.Utils;

namespace Bookstore.Web.Modules.NV1_Account.Services
{
    public class AuthService : IAuthService
    {
        private static AuthService? _instance;
        private static readonly object _lock = new object();
        
        // Khai báo các cổng dữ liệu an toàn
        private IUserRepository? _userRepository;

        public User? CurrentLoggedInUser { get; private set; }

        private AuthService() { }

        public static AuthService Instance
        {
            get
            {
                lock (_lock)
                {
                    if (_instance == null) _instance = new AuthService();
                    return _instance;
                }
            }
        }

        // HÀM CÀI ĐẶT: Giúp Program.cs cấu hình Repository cho Singleton runtime
        public void Initialize(IUserRepository userRepository)
        {
            _userRepository = userRepository;
        }

        private IUserRepository GetRepo() => _userRepository ?? throw new Exception("AuthService chưa được khởi tạo Repository!");

        public bool Login(string username, string password)
        {
            var user = GetRepo().GetByUsername(username);
            if (user == null) return false;

            bool isValid = PasswordHasher.VerifyPassword(password, user.PasswordHash, user.PasswordSalt);
            if (isValid)
            {
                CurrentLoggedInUser = user;
                return true;
            }
            return false;
        }

        public void Logout() => CurrentLoggedInUser = null;

        public bool Register(string username, string password)
        {
            if (GetRepo().GetByUsername(username) != null)
                return false;

            string hash = PasswordHasher.HashPassword(password, out string salt);
            
            var newUser = new User
            {
                Id = GetRepo().GetAll().Count + 1,
                Username = username,
                PasswordHash = hash,
                PasswordSalt = salt,
                Role = "Customer"
            };
            
            GetRepo().Add(newUser);
            return true;
        }

        public bool UpdateProfile(string newPassword)
        {
            if (CurrentLoggedInUser == null) return false;
            
            var user = GetRepo().GetById(CurrentLoggedInUser.Id);
            if (user != null)
            {
                string hash = PasswordHasher.HashPassword(newPassword, out string salt);
                user.PasswordHash = hash;
                user.PasswordSalt = salt;
                GetRepo().Update(user);
                return true;
            }
            return false;
        }

        public string GetCurrentUserRole()
        {
            return CurrentLoggedInUser?.Role ?? "Guest";
        }
    }
}