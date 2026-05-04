using Microsoft.EntityFrameworkCore;
using MovieService.Data;
using MovieService.Services;
using Microsoft.AspNetCore.Server.Kestrel.Core;

var builder = WebApplication.CreateBuilder(args);

// 1. Cấu hình Kestrel để chạy gRPC (HTTP/2) trên cổng 8080 - PHẢI ĐẶT Ở ĐÂY
builder.WebHost.ConfigureKestrel(options =>
{
    options.ListenAnyIP(8080, listenOptions =>
    {
        // Cho phép cả HTTP/1 (Web) và HTTP/2 (gRPC) chung cổng 8080
        listenOptions.Protocols = HttpProtocols.Http2;
    });
});

// 2. Add services to the container.
builder.Services.AddGrpc();
builder.Services.AddDbContext<MovieDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("MovieConnection")));

var app = builder.Build(); // Sau dòng này không được cấu hình thêm Service nữa

// 3. Configure the HTTP request pipeline.
app.MapGrpcService<MovieGrpcService>();
app.MapGet("/", () => "Movie gRPC Service is running on port 8080");

app.Run();