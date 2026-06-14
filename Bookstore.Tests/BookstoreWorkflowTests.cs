using System;
using System.Net;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.Testing;
using Xunit;
using Bookstore.Core.Models;
using Bookstore.Core.Models.NV2_Book;
using Bookstore.Core.Models.NV3_Cart;
using Bookstore.Web.Modules.NV1_Account.Services;

namespace Bookstore.Tests
{
    public class BookstoreWorkflowTests : IClassFixture<WebApplicationFactory<Program>>
    {
        private readonly HttpClient _client;

        public BookstoreWorkflowTests(WebApplicationFactory<Program> factory)
        {
            var CustomizedFactory = factory.WithWebHostBuilder(builder =>
            {
                builder.ConfigureServices(services =>
                {
                    if (MockDataStore.Books == null || MockDataStore.Books.Count == 0)
                    {
                        MockDataStore.Books = new System.Collections.Generic.List<Book>
                        {
                            new PaperBook { Id = 1, CategoryId = 1, Title = "Design Patterns cơ bản", Author = "Gang of Four", BasePrice = 150000, StockQuantity = 10 },
                            new EBook { Id = 2, CategoryId = 1, Title = "C# Nâng Cao và Clean Code", Author = "Tác giả Việt", BasePrice = 100000, StockQuantity = 99 }
                        };
                    }
                });
            });

            _client = CustomizedFactory.CreateClient();
        }

        private void EnsureUserHasCart(int userId)
        {
            if (!MockDataStore.UserCarts.ContainsKey(userId))
            {
                MockDataStore.UserCarts[userId] = new Cart();
            }
        }

        private void ResetSession()
        {
            AuthService.Instance.Logout();
        }

        private async Task SwitchToStaffSession()
        {
            string staffUser = "staff_manager";
            await _client.PostAsync($"/api/Account/register?username={staffUser}&password=staff123", null);
            
            var userInDb = MockDataStore.Users.Find(u => u.Username == staffUser);
            if (userInDb != null) { userInDb.Role = "STAFF"; }

            await _client.PostAsync($"/api/Account/login?username={staffUser}&password=staff123", null);
        }

        /// <summary>
        /// KỊCH BẢN 1: MUA HÀNG THANH TOÁN COD THÀNH CÔNG
        /// </summary>
        [Fact]
        public async Task Test_COD_Workflow_Success()
        {
            ResetSession();
            string username = $"cod_{Guid.NewGuid().ToString().Substring(0, 5)}";

            await _client.PostAsync($"/api/Account/register?username={username}&password=password123", null);
            await _client.PostAsync($"/api/Account/login?username={username}&password=password123", null);

            var user = MockDataStore.Users.Find(u => u.Username == username);
            Assert.NotNull(user);
            EnsureUserHasCart(user.Id);

            await _client.PostAsync("/api/Cart/add-item?bookId=1&quantity=1", null);

            var createOrderResponse = await _client.PostAsync("/api/Order/create-order?distanceInKm=3&applyVat=true&applyGiftWrapping=false", null);
            Assert.Equal(HttpStatusCode.OK, createOrderResponse.StatusCode);
            
            var orderContent = await createOrderResponse.Content.ReadAsStringAsync();
            using var doc = JsonDocument.Parse(orderContent);
            int orderId = doc.RootElement.GetProperty("orderId").GetInt32();

            var paymentResponse = await _client.PostAsync($"/api/Payment/execute-payment?orderId={orderId}&paymentMethod=COD", null);
            Assert.Equal(HttpStatusCode.OK, paymentResponse.StatusCode);

            await SwitchToStaffSession();

            var shipResponse = await _client.PutAsync($"/api/Order/{orderId}/process-next-step?action=proceed", null);
            Assert.Equal(HttpStatusCode.OK, shipResponse.StatusCode);

            var deliverResponse = await _client.PutAsync($"/api/Order/{orderId}/process-next-step?action=proceed", null);
            Assert.Equal(HttpStatusCode.OK, deliverResponse.StatusCode);
            
            var finalContent = await deliverResponse.Content.ReadAsStringAsync();
            Assert.Contains("đơn hàng thành công", finalContent);
        }

        /// <summary>
        /// KỊCH BẢN 2: THỬ THÁCH CHẶN LOGIC KHÔNG ĐƯỢC PHÉP SHIP KHI CHƯA QUA NV5
        /// </summary>
        [Fact]
        public async Task Test_OnlinePayment_Unpaid_Should_Block_Shipping()
        {
            ResetSession();
            string username = $"unpaid_{Guid.NewGuid().ToString().Substring(0, 5)}";

            await _client.PostAsync($"/api/Account/register?username={username}&password=password123", null);
            await _client.PostAsync($"/api/Account/login?username={username}&password=password123", null);
            
            var user = MockDataStore.Users.Find(u => u.Username == username);
            Assert.NotNull(user);
            EnsureUserHasCart(user.Id);

            await _client.PostAsync("/api/Cart/add-item?bookId=2&quantity=1", null);

            var createRes = await _client.PostAsync("/api/Order/create-order?distanceInKm=5&applyVat=true&applyGiftWrapping=true", null);
            Assert.Equal(HttpStatusCode.OK, createRes.StatusCode); 
            
            var content = await createRes.Content.ReadAsStringAsync();
            using var doc = JsonDocument.Parse(content);
            int orderId = doc.RootElement.GetProperty("orderId").GetInt32();

            var failedShipResponse = await _client.PutAsync($"/api/Order/{orderId}/process-next-step?action=proceed", null);
            
            Assert.Equal(HttpStatusCode.Forbidden, failedShipResponse.StatusCode);
        }

        /// <summary>
        /// ✨ KỊCH BẢN 3 ĐÃ SỬA ĐỔI: LUỒNG MUA HÀNG ONLINE (MOMO) THÀNH CÔNG, GIAO HÀNG VÀ BÁN ĐƯỢC SÁCH
        /// </summary>
        [Fact]
        public async Task Test_OnlinePayment_MOMO_Workflow_Success()
        {
            ResetSession();
            string username = $"momo_{Guid.NewGuid().ToString().Substring(0, 5)}";

            // 1. Khách hàng đăng ký & đăng nhập
            await _client.PostAsync($"/api/Account/register?username={username}&password=password123", null);
            await _client.PostAsync($"/api/Account/login?username={username}&password=password123", null);
            
            var user = MockDataStore.Users.Find(u => u.Username == username);
            Assert.NotNull(user);
            EnsureUserHasCart(user.Id);

            // 2. Thêm sách vào giỏ hàng
            await _client.PostAsync("/api/Cart/add-item?bookId=1&quantity=1", null);

            // 3. Tạo đơn hàng thô
            var createRes = await _client.PostAsync("/api/Order/create-order?distanceInKm=2&applyVat=false&applyGiftWrapping=false", null);
            Assert.Equal(HttpStatusCode.OK, createRes.StatusCode);
            
            var content = await createRes.Content.ReadAsStringAsync();
            using var doc = JsonDocument.Parse(content);
            int orderId = doc.RootElement.GetProperty("orderId").GetInt32();

            // 4. Thanh toán trực tuyến qua cổng MOMO (Xác thực Paid, tự đẩy lên PendingState)
            var paymentResponse = await _client.PostAsync($"/api/Payment/execute-payment?orderId={orderId}&paymentMethod=MOMO", null);
            Assert.Equal(HttpStatusCode.OK, paymentResponse.StatusCode);

            // 5. Chuyển sang quyền STAFF để xử lý đóng gói và vận chuyển vật lý
            await SwitchToStaffSession();

            // Giao hàng: Chuyển từ PendingState -> DeliveringState
            var shipResponse = await _client.PutAsync($"/api/Order/{orderId}/process-next-step?action=proceed", null);
            Assert.Equal(HttpStatusCode.OK, shipResponse.StatusCode);

            // Hoàn tất: Chuyển từ DeliveringState -> DeliveredState (Bán sách thành công)
            var deliverResponse = await _client.PutAsync($"/api/Order/{orderId}/process-next-step?action=proceed", null);
            Assert.Equal(HttpStatusCode.OK, deliverResponse.StatusCode);

            // 6. Kiểm tra kết quả hệ thống trả về
            var finalContent = await deliverResponse.Content.ReadAsStringAsync();
            Assert.Contains("đơn hàng thành công", finalContent);
            
            // (Lúc này dưới Console của dotnet test sẽ in ra log Observer cộng điểm và trừ kho của đơn hàng MoMo này tương tự COD)
        }

        /// <summary>
        /// KỊCH BẢN 4: KIỂM TRA BẢO MẬT MẬT KHẨU MÃ HÓA HASH (NV1)
        /// </summary>
        [Fact]
        public async Task Test_Login_With_Wrong_Password_Should_Fail()
        {
            ResetSession();
            string username = $"secure_{Guid.NewGuid().ToString().Substring(0, 5)}";

            await _client.PostAsync($"/api/Account/register?username={username}&password=correct_pass", null);

            var loginResponse = await _client.PostAsync($"/api/Account/login?username={username}&password=wrong_pass", null);
            
            Assert.Equal(HttpStatusCode.BadRequest, loginResponse.StatusCode);
            var content = await loginResponse.Content.ReadAsStringAsync();
            Assert.Contains("Sai tài khoản hoặc mật khẩu!", content);
        }
    }
}