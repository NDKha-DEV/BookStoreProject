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
                    // Đảm bảo dữ liệu Sách mẫu luôn có sẵn trong lõi hệ thống ngầm
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

        /// <summary>
        /// KỊCH BẢN 1: MUA HÀNG THANH TOÁN COD THÀNH CÔNG
        /// </summary>
        [Fact]
        public async Task Test_COD_Workflow_Success()
        {
            string username = $"cod_{Guid.NewGuid().ToString().Substring(0, 5)}";

            await _client.PostAsync($"/api/Account/register?username={username}&password=password123", null);
            await _client.PostAsync($"/api/Account/login?username={username}&password=password123", null);

            var user = MockDataStore.Users.Find(u => u.Username == username);
            Assert.NotNull(user);
            EnsureUserHasCart(user.Id);

            await _client.PostAsync("/api/Cart/add-item?bookId=1&quantity=1", null);

            // Bước 2 chuẩn kịch bản: Gọi API tạo đơn thô không truyền phương thức thanh toán
            var createOrderResponse = await _client.PostAsync("/api/Order/create-order?distanceInKm=3&applyVat=true&applyGiftWrapping=false", null);
            Assert.Equal(HttpStatusCode.OK, createOrderResponse.StatusCode);
            
            var orderContent = await createOrderResponse.Content.ReadAsStringAsync();
            using var doc = JsonDocument.Parse(orderContent);
            int orderId = doc.RootElement.GetProperty("orderId").GetInt32(); // Chuẩn chữ thường 'orderId'

            // Bước 3 chuẩn kịch bản: Gọi sang phân hệ NV5 để chọn cổng COD và chuyển tiếp State đơn hàng
            var paymentResponse = await _client.PostAsync($"/api/Payment/execute-payment?orderId={orderId}&paymentMethod=COD", null);
            Assert.Equal(HttpStatusCode.OK, paymentResponse.StatusCode);

            // Kịch bản 3: Admin và Shipper tiến hành các bước xử lý vật lý kế tiếp
            var shipResponse = await _client.PutAsync($"/api/Order/{orderId}/process-next-step?action=proceed", null);
            Assert.Equal(HttpStatusCode.OK, shipResponse.StatusCode);

            var deliverResponse = await _client.PutAsync($"/api/Order/{orderId}/process-next-step?action=proceed", null);
            Assert.Equal(HttpStatusCode.OK, deliverResponse.StatusCode);
            
            var finalContent = await deliverResponse.Content.ReadAsStringAsync();
            Assert.Contains("Thao tác duyệt trạng thái thành công!", finalContent);
        }

        /// <summary>
        /// KỊCH BẢN 2: THỬ THÁCH CHẶN LOGIC (DUYỆT ĐƠN KHI CHƯA QUA KHÂU NV5 ĐỂ CHỌN CỔNG/THANH TOÁN)
        /// </summary>
        [Fact]
        public async Task Test_OnlinePayment_Unpaid_Should_Block_Shipping()
        {
            string username = $"unpaid_{Guid.NewGuid().ToString().Substring(0, 5)}";

            await _client.PostAsync($"/api/Account/register?username={username}&password=password123", null);
            await _client.PostAsync($"/api/Account/login?username={username}&password=password123", null);
            
            var user = MockDataStore.Users.Find(u => u.Username == username);
            Assert.NotNull(user);
            EnsureUserHasCart(user.Id);

            await _client.PostAsync("/api/Cart/add-item?bookId=2&quantity=1", null);

            // Tạo đơn thô (PaymentMethod tự động ăn theo thiết kế = "PENDING")
            var createRes = await _client.PostAsync("/api/Order/create-order?distanceInKm=5&applyVat=true&applyGiftWrapping=true", null);
            Assert.Equal(HttpStatusCode.OK, createRes.StatusCode); 
            
            var content = await createRes.Content.ReadAsStringAsync();
            using var doc = JsonDocument.Parse(content);
            int orderId = doc.RootElement.GetProperty("orderId").GetInt32();

            // Cố tình bỏ qua phân hệ NV5, yêu cầu NV4 duyệt đơn luôn -> State Pattern sẽ quăng Exception chặn đứng
            var failedShipResponse = await _client.PutAsync($"/api/Order/{orderId}/process-next-step?action=proceed", null);
            
            Assert.Equal(HttpStatusCode.BadRequest, failedShipResponse.StatusCode);
            var errContent = await failedShipResponse.Content.ReadAsStringAsync();
            Assert.Contains("chưa được thanh toán thành công", errContent);
        }

        /// <summary>
        /// KỊCH BẢN 3: LUỒNG MUA HÀNG THANH TOÁN ONLINE THÀNH CÔNG VÀ TIẾN HÀNH HỦY ĐƠN (REFUND)
        /// </summary>
        [Fact]
        public async Task Test_Cancel_Paid_Order_Should_Refund_And_Trigger_CancelObserver()
        {
            string username = $"cancel_{Guid.NewGuid().ToString().Substring(0, 5)}";

            await _client.PostAsync($"/api/Account/register?username={username}&password=password123", null);
            await _client.PostAsync($"/api/Account/login?username={username}&password=password123", null);
            
            var user = MockDataStore.Users.Find(u => u.Username == username);
            Assert.NotNull(user);
            EnsureUserHasCart(user.Id);

            await _client.PostAsync("/api/Cart/add-item?bookId=1&quantity=1", null);

            // Tạo đơn thô
            var createRes = await _client.PostAsync("/api/Order/create-order?distanceInKm=2&applyVat=false&applyGiftWrapping=false", null);
            Assert.Equal(HttpStatusCode.OK, createRes.StatusCode);
            
            var content = await createRes.Content.ReadAsStringAsync();
            using var doc = JsonDocument.Parse(content);
            int orderId = doc.RootElement.GetProperty("orderId").GetInt32();

            // Gọi sang NV5 chọn chiến lược MOMO, xử lý trừ tiền ảo và tự động kích hoạt đẩy State lên PendingState công khai
            var paymentResponse = await _client.PostAsync($"/api/Payment/execute-payment?orderId={orderId}&paymentMethod=MOMO", null);
            Assert.Equal(HttpStatusCode.OK, paymentResponse.StatusCode);

            // Người dùng phát lệnh hủy đơn khi đang ở trạng thái PendingState
            var cancelResponse = await _client.PutAsync($"/api/Order/{orderId}/process-next-step?action=cancel", null);
            Assert.Equal(HttpStatusCode.OK, cancelResponse.StatusCode);

            var cancelContent = await cancelResponse.Content.ReadAsStringAsync();
            Assert.Contains("Thao tác duyệt trạng thái thành công!", cancelContent);
        }

        /// <summary>
        /// KỊCH BẢN 4: KIỂM TRA BẢO MẬT MẬT KHẨU MÃ HÓA HASH (NV1)
        /// </summary>
        [Fact]
        public async Task Test_Login_With_Wrong_Password_Should_Fail()
        {
            string username = $"secure_{Guid.NewGuid().ToString().Substring(0, 5)}";

            await _client.PostAsync($"/api/Account/register?username={username}&password=correct_pass", null);

            var loginResponse = await _client.PostAsync($"/api/Account/login?username={username}&password=wrong_pass", null);
            
            Assert.Equal(HttpStatusCode.BadRequest, loginResponse.StatusCode);
            var content = await loginResponse.Content.ReadAsStringAsync();
            Assert.Contains("Sai tài khoản hoặc mật khẩu!", content);
        }
    }
}