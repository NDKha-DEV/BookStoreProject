using Bookstore.Core.Interfaces;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Bookstore.Web.Services;
using Microsoft.AspNetCore.Http;
using System.Text.Json.Serialization;

var builder = WebApplication.CreateBuilder(args);

// -----------------------------------------------------------------------------
// 🛠️ KHU VỰC CẤU HÌNH DEPENDENCY INJECTION (DI CONTAINER)
// Nơi Trưởng nhóm đấu nối Interface với các Class Pattern cụ thể của thành viên
// -----------------------------------------------------------------------------

// 1. Cấu hình các Controllers để hứng Request từ Frontend/Postman thay vì dùng Minimal API mặc định
builder.Services.AddControllers().AddJsonOptions(opts => {
    opts.JsonSerializerOptions.DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull;
});

// Session and in-memory cache for storing cart per-user
builder.Services.AddDistributedMemoryCache();
builder.Services.AddSession();

builder.Services.AddHttpContextAccessor();

// App services for cart and inventory
builder.Services.AddScoped<SessionCartManager>();
builder.Services.AddScoped<ICartManager, CartValidationDecorator>();

builder.Services.AddSingleton<IInventoryStrategy, InMemoryInventoryStrategy>();
builder.Services.AddSingleton<IInventoryService, InventoryService>();

// 2. Cấu hình Swagger/OpenAPI (để test API trực quan trên trình duyệt)
builder.Services.AddOpenApi();


// 💡 Gợi ý đấu nối cho các module tiếp theo (khi các thành viên code xong, bạn hãy bỏ comment ra):
// builder.Services.AddSingleton<IAuthService, AuthService>();     // NV1 - Singleton vì quản lý session chung
// builder.Services.AddScoped<IBookService, BookService>();         // NV2
// builder.Services.AddScoped<ICartService, CartService>();         // NV3
// builder.Services.AddScoped<IOrderService, OrderService>();       // NV4
// builder.Services.AddScoped<IPaymentService, PaymentService>();   // NV5


var app = builder.Build();

// -----------------------------------------------------------------------------
// 🌐 KHU VỰC CẤU HÌNH HTTP REQUEST PIPELINE (MIDDLEWARE)
// -----------------------------------------------------------------------------

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    
    // Swagger UI không khả dụng với package Microsoft.AspNetCore.OpenApi hiện tại.
}

app.UseHttpsRedirection();

// Kích hoạt tính năng Routing để map các Request vào các file Controller của nhóm
app.UseRouting();

// Enable session middleware so CartService can use session storage
app.UseSession();

// Thêm Middleware kiểm tra quyền truy cập (Bổ trợ trực tiếp cho Proxy Pattern của NV1)
app.UseAuthorization();

// Đăng ký các Endpoint từ Controllers
app.MapControllers();

app.Run();