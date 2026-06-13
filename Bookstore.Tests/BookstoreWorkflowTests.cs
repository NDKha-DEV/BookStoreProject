// Vị trí: Bookstore.Tests/BookstoreWorkflowTests.cs
using System;
using System.Net;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.Testing;
using Xunit;

namespace Bookstore.Tests
{
    public class BookstoreWorkflowTests : IClassFixture<WebApplicationFactory<Program>>
    {
        private readonly HttpClient _client;

        public BookstoreWorkflowTests(WebApplicationFactory<Program> factory)
        {
            // Giả lập một Client kết nối trực tiếp đến Web API chạy trong bộ nhớ RAM
            _client = factory.CreateClient();
        }

        /// <summary>
        /// KỊCH BẢN 1: MUA HÀNG THANH TOÁN COD THÀNH CÔNG (LUỒNG SHOPEE)
        /// </summary>
        [Fact]
        public async Task Test_COD_Workflow_Success()
        {
            string username = $"cod_{Guid.NewGuid().ToString().Substring(0, 5)}";

            // 1. Đăng ký tài khoản
            var reg = await _client.PostAsync($"/api/Account/register?username={username}&password=password123", null);
            Assert.Equal(HttpStatusCode.OK, reg.StatusCode);

            // 2. Đăng nhập để kích hoạt session đồng bộ cho AuthService
            var login = await _client.PostAsync($"/api/Account/login?username={username}&password=password123", null);
            Assert.Equal(HttpStatusCode.OK, login.StatusCode);

            // 3. Thêm sách vào giỏ hàng thực tế để tạo tiền hàng gốc động (> 0)
            var addCart = await _client.PostAsync("/api/Cart/add-item?bookId=1&quantity=1", null);
            Assert.Equal(HttpStatusCode.OK, addCart.StatusCode);

            // 4. Tạo đơn hàng COD
            var createOrderResponse = await _client.PostAsync("/api/Order/create-order?paymentMethod=COD&distanceInKm=3&usePremiumPackaging=false", null);
            Assert.Equal(HttpStatusCode.OK, createOrderResponse.StatusCode);
            
            var orderContent = await createOrderResponse.Content.ReadAsStringAsync();
            using var doc = JsonDocument.Parse(orderContent);
            int orderId = doc.RootElement.GetProperty("orderId").GetInt32(); // ✨ LẤY ID ĐỘNG TỪ API

            // 5. Admin duyệt đơn sang trạng thái Đang giao hàng (Shipping)
            var shipResponse = await _client.PutAsync($"/api/Order/{orderId}/process-next-step?action=proceed", null);
            Assert.Equal(HttpStatusCode.OK, shipResponse.StatusCode);

            // 6. Shipper hoàn tất giao hàng -> Trigger Observer cấu trừ kho và cộng điểm thành viên
            var deliverResponse = await _client.PutAsync($"/api/Order/{orderId}/process-next-step?action=proceed", null);
            Assert.Equal(HttpStatusCode.OK, deliverResponse.StatusCode);
            
            var finalContent = await deliverResponse.Content.ReadAsStringAsync();
            Assert.Contains("Thao tác duyệt trạng thái thành công!", finalContent);
        }

        /// <summary>
        /// KỊCH BẢN 2: THỬ THÁCH CHẶN LOGIC (RÀNG BUỘC THANH TOÁN ONLINE)
        /// </summary>
        [Fact]
        public async Task Test_OnlinePayment_Unpaid_Should_Block_Shipping()
        {
            string username = $"unpaid_{Guid.NewGuid().ToString().Substring(0, 5)}";

            await _client.PostAsync($"/api/Account/register?username={username}&password=password123", null);
            await _client.PostAsync($"/api/Account/login?username={username}&password=password123", null);
            await _client.PostAsync("/api/Cart/add-item?bookId=2&quantity=1", null);

            // Tạo đơn ONLINE nhưng cố tình KHÔNG gọi sang cổng thanh toán Adapter (Tiền vẫn Unpaid)
            var createRes = await _client.PostAsync("/api/Order/create-order?paymentMethod=ONLINE&distanceInKm=5&usePremiumPackaging=true", null);
            var content = await createRes.Content.ReadAsStringAsync();
            using var doc = JsonDocument.Parse(content);
            int orderId = doc.RootElement.GetProperty("orderId").GetInt32();

            // Cố tình gọi lệnh tiến hành giao hàng khi chưa trả tiền trực tuyến
            var failedShipResponse = await _client.PutAsync($"/api/Order/{orderId}/process-next-step?action=proceed", null);
            
            // State Pattern chặn và ném về Bad Request (400) thỏa mãn điều kiện logic của nhóm
            Assert.Equal(HttpStatusCode.BadRequest, failedShipResponse.StatusCode);
            var errContent = await failedShipResponse.Content.ReadAsStringAsync();
            Assert.Contains("chưa được thanh toán thành công", errContent);
        }

        /// <summary>
        /// KỊCH BẢN 3: LUỒNG HỦY ĐƠN ĐÃ THANH TOÁN (HOÀN TIỀN - REFUND)
        /// </summary>
        [Fact]
        public async Task Test_Cancel_Paid_Order_Should_Refund_And_Trigger_CancelObserver()
        {
            string username = $"cancel_{Guid.NewGuid().ToString().Substring(0, 5)}";

            await _client.PostAsync($"/api/Account/register?username={username}&password=password123", null);
            await _client.PostAsync($"/api/Account/login?username={username}&password=password123", null);
            await _client.PostAsync("/api/Cart/add-item?bookId=1&quantity=1", null);

            // 1. Tạo đơn ONLINE
            var createRes = await _client.PostAsync("/api/Order/create-order?paymentMethod=ONLINE&distanceInKm=2&usePremiumPackaging=false", null);
            var content = await createRes.Content.ReadAsStringAsync();
            using var doc = JsonDocument.Parse(content);
            int orderId = doc.RootElement.GetProperty("orderId").GetInt32();

            // 2. Giả lập cổng thanh toán điện tử xác nhận quét mã thành công (Chuyển trạng thái sang Paid)
            await _client.PostAsync($"/api/Payment/execute-online-payment?orderId={orderId}&gateway=vnpay", null);

            // 3. Người dùng phát lệnh HỦY đơn ngay tại trạng thái Chờ duyệt đóng gói
            var cancelResponse = await _client.PutAsync($"/api/Order/{orderId}/process-next-step?action=cancel", null);
            Assert.Equal(HttpStatusCode.OK, cancelResponse.StatusCode);

            var cancelContent = await cancelResponse.Content.ReadAsStringAsync();
            Assert.Contains("Thao tác duyệt trạng thái thành công!", cancelContent);
        }

        /// <summary>
        /// KỊCH BẢN 4: KIỂM TRA BẢO MẬT MẬT KHẨU MÃ HÓA HASH
        /// </summary>
        [Fact]
        public async Task Test_Login_With_Wrong_Password_Should_Fail()
        {
            string username = $"secure_{Guid.NewGuid().ToString().Substring(0, 5)}";

            // Đăng ký với mật khẩu gốc chuẩn chỉnh
            await _client.PostAsync($"/api/Account/register?username={username}&password=correct_pass", null);

            // Thử thách đăng nhập bằng mật khẩu sai lệch hoàn toàn
            var loginResponse = await _client.PostAsync($"/api/Account/login?username={username}&password=wrong_pass", null);
            
            // Trình bảo mật băm SHA256 đối soát thấy không khớp và chặn đứng bằng Http 400 Bad Request
            Assert.Equal(HttpStatusCode.BadRequest, loginResponse.StatusCode);
            var content = await loginResponse.Content.ReadAsStringAsync();
            Assert.Contains("Sai tài khoản hoặc mật khẩu!", content);
        }
    }
}