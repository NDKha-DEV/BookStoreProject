// Vị trí: Bookstore.Web/Program.cs
using Bookstore.Core.Interfaces;
using Bookstore.Core.Models.NV2_Book;
using Bookstore.Core.Models.NV4_Order.Interfaces;
using Bookstore.Core.Security;
using Bookstore.Core.Models.NV1_Account;
using Bookstore.Web.Modules.NV1_Account.Services;
using Bookstore.Web.Modules.NV2_Book.Services;
using Bookstore.Web.Modules.NV4_Order.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAllFrontend",
        policy => policy.AllowAnyOrigin()
                        .AllowAnyMethod()
                        .AllowAnyHeader());
});
// -----------------------------------------------------------------------------
// 🛠️ KHU VỰC CẤU HÌNH DEPENDENCY INJECTION (DI CONTAINER)
// -----------------------------------------------------------------------------

// 1. Đăng ký bộ thu nạp Controller API hứng nhận Request từ Frontend / Swagger UI
builder.Services.AddControllers();

// 2. Đăng ký cấu hình tự động tạo tài liệu API Swagger/OpenAPI v1
builder.Services.AddOpenApi();

// 3. ĐẤU NỐI CÁC MODULE CHUYÊN BIỆT THEO CHUẨN DESIGN PATTERN
// [NV1]: Đăng ký Strategy (Nếu muốn đổi thuật toán, CHỈ CẦN SỬA ĐÚNG DÒNG NÀY)
// builder.Services.AddSingleton<IPasswordHashStrategy, BcryptPasswordStrategy>();
builder.Services.AddSingleton<IPasswordHashStrategy, Sha256PasswordStrategy>();

// AuthService lưu trạng thái đăng nhập hiện tại trong bộ nhớ giả lập.
// Vì demo chưa dùng session/cookie, đăng ký singleton để các request liên tiếp cùng nhận cùng phiên.
builder.Services.AddSingleton<IAuthService, AuthService>();
builder.Services.AddScoped<AccountService>();
builder.Services.AddScoped<AccountProxy>();

// [NV2]: Đăng ký dịch vụ sách thông qua giao diện Interface chuẩn trừu tượng
builder.Services.AddScoped<IBookService, BookService>(); 

builder.Services.AddScoped<CategoryService>();

// [NV4]: Đăng ký lõi quản lý vòng đời đơn hàng xử lý State và Observer
builder.Services.AddScoped<IOrderService, OrderService>();

// (Lưu ý kiến trúc: NV3 và NV5 thực thi cấu trúc cơ động Decorator/Strategy/Adapter 
// trực tiếp tại Runtime luồng xử lý của Controller nên không cần ép đăng ký Service).

var app = builder.Build();

// -----------------------------------------------------------------------------
// 🌐 KHU VỰC CẤU HÌNH HTTP REQUEST PIPELINE (MIDDLEWARE ĐIỀU PHỐI)
// -----------------------------------------------------------------------------

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    
    // Bật giao diện Swagger UI giúp nhóm dễ dàng test bấm nút demo trực tiếp trước Hội đồng
    app.UseSwaggerUI(options =>
    {
        options.SwaggerEndpoint("/openapi/v1.json", "E-Commerce Bookstore API v1");
    }); 
}

app.UseCors("AllowAllFrontend");

app.UseHttpsRedirection();

// Kích hoạt Middleware định tuyến liên kết URL khớp với các Controller
app.UseRouting();

// Middleware hỗ trợ phân quyền bổ trợ cho mẫu thiết kế Proxy Pattern bảo mật
app.UseAuthorization();

// Đăng ký ánh xạ các Endpoint API cấu hình trong các Controller
app.MapControllers();

app.Run();

public partial class Program { }