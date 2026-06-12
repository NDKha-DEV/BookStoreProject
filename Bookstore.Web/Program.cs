using Bookstore.Core.Interfaces;
using Bookstore.Web.Modules.NV2_Book.Services;
var builder = WebApplication.CreateBuilder(args);
// Đăng ký dịch vụ
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer(); 
builder.Services.AddSwaggerGen(); 

builder.Services.AddScoped<IBookService, BookService>();
builder.Services.AddSingleton<CategoryService>();
var app = builder.Build();
// Cấu hình Middleware
if (app.Environment.IsDevelopment())
{
    app.UseSwagger(); 
    app.UseSwaggerUI(); 
}

app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();

app.Run();