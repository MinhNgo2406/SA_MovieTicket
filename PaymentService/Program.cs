using Microsoft.EntityFrameworkCore;
using PaymentService.Data;

var builder = WebApplication.CreateBuilder(args);

// 1. ??ng ký các d?ch v? h? th?ng
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// 2. C?u hình Database
builder.Services.AddDbContext<PaymentDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// 3. N?u b?n có dùng RabbitMQ thì ??ng ký thêm ? ?ây
// builder.Services.AddSingleton<RabbitMQPublisher>(); 

var app = builder.Build();

// 4. C?u hình Pipeline (Th? t? r?t quan tr?ng)
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(); // ??a vào trong Development ?? b?o m?t khi deploy
}

app.UseHttpsRedirection();

app.UseAuthorization(); // Thêm dòng này n?u sau này b?n làm phân quy?n

app.MapControllers(); // ?ây là dòng ?? nó nh?n các Controller trong folder Controllers

app.Run();