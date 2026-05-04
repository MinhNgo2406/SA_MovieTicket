using Grpc.Net.Client;
using Microsoft.AspNetCore.Mvc;
using MovieService; // Namespace của gRPC Client (sinh ra từ proto)
using MovieTicket.Models;
using System.Diagnostics;

namespace MovieTicket.Controllers;

public class HomeController : Controller
{
    private readonly MovieBookingContext _ctx;

    public HomeController(MovieBookingContext ctx)
    {
        _ctx = ctx;
    }

    public async Task<IActionResult> Index(string search)
    {
        // Cho phép HTTP/2 không bảo mật (cần thiết cho Docker)
        AppContext.SetSwitch("System.Net.Http.SocketsHttpHandler.Http2UnencryptedSupport", true);

        var movies = new List<MovieReply>();

        try
        {
            // Sử dụng "using" để tự động giải phóng kênh kết nối sau khi dùng xong
            using var channel = GrpcChannel.ForAddress("http://movie-service:8080");
            var client = new MovieGrpc.MovieGrpcClient(channel);

            // Gọi API gRPC
            using var call = client.GetAllMovies(new Empty());

            while (await call.ResponseStream.MoveNext(CancellationToken.None))
            {
                var movie = call.ResponseStream.Current;

                // Lọc phim theo từ khóa tìm kiếm (nếu có)
                if (string.IsNullOrEmpty(search) || movie.Title.Contains(search, StringComparison.OrdinalIgnoreCase))
                {
                    movies.Add(movie);
                }
            }
        }
        catch (Exception ex)
        {
            // Log lỗi vào Console của Docker để bạn tiện kiểm tra
            Console.WriteLine($"[ERROR] gRPC Connection Failed: {ex.Message}");
            
            // Có thể truyền thông báo lỗi sang View nếu muốn
            ViewBag.ErrorMessage = "Không thể kết nối đến máy chủ phim. Vui lòng thử lại sau.";
        }

        // Trả về view với danh sách phim (có thể là rỗng nếu lỗi)
        return View(movies);
    }

    public IActionResult Privacy()
    {
        return View();
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel
        {
            RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier
        });
    }
}