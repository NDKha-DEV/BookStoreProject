using Bookstore.Core.Interfaces;
using Bookstore.Web.Modules.NV1_Account;
using Bookstore.Web.Modules.NV2_Book;
using Bookstore.Web.Modules.NV3_Cart;
using Bookstore.Web.Modules.NV4_Order;
using Bookstore.Web.Modules.NV5_Payment; 

var builder = WebApplication.CreateBuilder(args);

// -----------------------------------------------------------------------------
// 🛠️ KHU VỰC CẤU HÌNH DEPENDENCY INJECTION (DI CONTAINER)
// Nơi Trưởng nhóm đấu nối Interface với các Class Pattern cụ thể của thành viên
// -----------------------------------------------------------------------------

// 1. Cấu hình các Controllers để hứng Request từ Frontend/Postman thay vì dùng Minimal API mặc định
builder.Services.AddControllers();

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
    
    // Thêm dòng này nếu bạn muốn dùng giao diện Swagger UI trực quan để test API (Rất điểm cộng khi báo cáo)
    app.UseSwaggerUI(); 
}

app.UseHttpsRedirection();

// Kích hoạt tính năng Routing để map các Request vào các file Controller của nhóm
app.UseRouting();

// Thêm Middleware kiểm tra quyền truy cập (Bổ trợ trực tiếp cho Proxy Pattern của NV1)
app.UseAuthorization();

// Đăng ký các Endpoint từ Controllers
app.MapControllers();

app.Run();