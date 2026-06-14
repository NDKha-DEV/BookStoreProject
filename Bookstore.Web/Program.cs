// Vị trí: Bookstore.Web/Program.cs
using Bookstore.Core.Interfaces;
using Bookstore.Infrastructure.Repositories;
using Bookstore.Web.Modules.NV1_Account.Services;
using Bookstore.Core.Models.NV2_Book;
using Bookstore.Core.Models.NV4_Order.Interfaces;
using Bookstore.Web.Modules.NV2_Book.Services;
using Bookstore.Web.Modules.NV4_Order.Services;
using Bookstore.Web.Modules.NV5_Payment.Services;

var builder = WebApplication.CreateBuilder(args);

// -----------------------------------------------------------------------------
// 🛠️ KHU VỰC CẤU HÌNH DEPENDENCY INJECTION (DI CONTAINER)
// -----------------------------------------------------------------------------

// 1. Đăng ký bộ thu nạp Controller API hứng nhận Request từ Frontend / Swagger UI
builder.Services.AddControllers();

// 2. Đăng ký cấu hình tự động tạo tài liệu API Swagger/OpenAPI v1
builder.Services.AddOpenApi();

var userRepo = new MockUserRepository(); 
AuthService.Instance.Initialize(userRepo);

builder.Services.AddSingleton<IUserRepository>(userRepo);

builder.Services.AddSingleton<IBookRepository, MockBookRepository>();
builder.Services.AddSingleton<ICategoryRepository, MockCategoryRepository>();
builder.Services.AddSingleton<ICartRepository, Bookstore.Infrastructure.Repositories.MockCartRepository>();
builder.Services.AddSingleton<IOrderRepository, MockOrderRepository>();
builder.Services.AddSingleton<IPaymentRepository, MockPaymentRepository>();

// 3. ĐẤU NỐI CÁC MODULE CHUYÊN BIỆT THEO CHUẨN DESIGN PATTERN
// [NV1]: AuthService quản lý Session Singleton có Instance nội bộ riêng, không nạp tại đây.

// [NV2]: Đăng ký dịch vụ sách thông qua giao diện Interface chuẩn trừu tượng
builder.Services.AddScoped<IBookService, BookService>(); 

builder.Services.AddScoped<ICategoryService, CategoryService>();

// [NV4]: Đăng ký lõi quản lý vòng đời đơn hàng xử lý State và Observer
builder.Services.AddScoped<IOrderService, OrderService>();

builder.Services.AddScoped<PaymentService>();

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

app.UseHttpsRedirection();

// Kích hoạt Middleware định tuyến liên kết URL khớp với các Controller
app.UseRouting();

// Middleware hỗ trợ phân quyền bổ trợ cho mẫu thiết kế Proxy Pattern bảo mật
app.UseAuthorization();

// Đăng ký ánh xạ các Endpoint API cấu hình trong các Controller
app.MapControllers();

app.Run();

public partial class Program { }