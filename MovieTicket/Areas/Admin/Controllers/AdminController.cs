using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using MovieTicket.Models;

namespace MovieTicket.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class AdminController : Controller
    {
        public MovieBookingContext _ctx;
        public AdminController(MovieBookingContext ctx)
        {
            _ctx = ctx;
        }
        public IActionResult Index()
        {
            // Tổng phim
            ViewBag.TotalMovies = _ctx.Movies.Count();

            // Tổng rạp
            ViewBag.TotalCinemas = _ctx.Cinemas.Count();

            // Tổng vé
            ViewBag.TotalTickets = _ctx.Tickets.Count();

            // Tổng user
            ViewBag.TotalUsers = _ctx.Users.Count();

            // Doanh thu
            ViewBag.TotalRevenue = _ctx.Tickets.Count() * 100000;

            // Top phim
            var topMovies = _ctx.Movies
                .Select(m => new
                {
                    m.Title,
                    m.Image,
                    BookingCount = m.Showtimes
                        .SelectMany(s => s.Tickets)
                        .Count()
                })
                .OrderByDescending(x => x.BookingCount)
                .Take(5)
                .ToList();

            ViewBag.TopMovies = topMovies;

            // Vé gần đây
            var recentTickets = _ctx.Tickets
                .OrderByDescending(x => x.Id)
                .Take(5)
                .Select(x => new
                {
                    x.Id,
                    MovieName = x.ShowTime.Movie.Title,
                    UserName = x.Order.User.Name,
                    SeatName = x.Seat.RowName + x.Seat.SeatNumber,
                    Price = 100000
                })
                .ToList();

            ViewBag.RecentTickets = recentTickets;

            return View();
        }
        // =========================
        // CREATE
        // =========================
        [HttpGet]
        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Create(Movie movie, IFormFile ImageFile)
        {
            Console.WriteLine(movie.Title);
            Console.WriteLine(movie.Duration);
            Console.WriteLine(movie.Rating);
            if (ImageFile != null)
            {
                string fileName = Guid.NewGuid().ToString()
                    + Path.GetExtension(ImageFile.FileName);

                string path = Path.Combine(
                    Directory.GetCurrentDirectory(),
                    "wwwroot/img",
                    fileName);

                using (var stream = new FileStream(path, FileMode.Create))
                {
                    ImageFile.CopyTo(stream);
                }

                movie.Image = fileName;
            }

            _ctx.Movies.Add(movie);
            _ctx.SaveChanges();

            return RedirectToAction("Movies");
        }

        // =========================
        // EDIT
        // =========================
        [HttpGet]
        public IActionResult Edit(int id)
        {
            var movie = _ctx.Movies.Find(id);

            if (movie == null)
            {
                return NotFound();
            }

            return View(movie);
        }

        [HttpPost]
        public IActionResult Edit(Movie movie, IFormFile? ImageFile)
        {
            var oldMovie = _ctx.Movies.Find(movie.Id);

            if (oldMovie == null)
            {
                return NotFound();
            }

            oldMovie.Title = movie.Title;
            oldMovie.Duration = movie.Duration;
            oldMovie.Rating = movie.Rating;
            oldMovie.Description = movie.Description;

            // upload ảnh mới
            if (ImageFile != null)
            {
                string fileName = Guid.NewGuid().ToString()
                                  + Path.GetExtension(ImageFile.FileName);

                string path = Path.Combine(
                    Directory.GetCurrentDirectory(),
                    "wwwroot/img",
                    fileName
                );

                using (var stream = new FileStream(path, FileMode.Create))
                {
                    ImageFile.CopyTo(stream);
                }

                oldMovie.Image = fileName;
            }

            _ctx.SaveChanges();

            TempData["Success"] = "✅ Update movie successfully";

            return RedirectToAction("Movies");
        }

        // =========================
        // DELETE
        // =========================
        [HttpGet]
        public IActionResult Delete(int id)
        {
            var movie = _ctx.Movies
                .FirstOrDefault(x => x.Id == id);

            if (movie == null)
            {
                return NotFound();
            }

            // Lấy tất cả showtime của movie
            var showtimes = _ctx.Showtimes
                .Where(x => x.MovieId == id)
                .ToList();

            // Xóa ticket liên quan
            foreach (var showtime in showtimes)
            {
                var tickets = _ctx.Tickets
                    .Where(t => t.ShowTimeId == showtime.Id)
                    .ToList();

                _ctx.Tickets.RemoveRange(tickets);
            }

            // Xóa reviews
            var reviews = _ctx.Reviews
                .Where(r => r.MovieId == id)
                .ToList();

            _ctx.Reviews.RemoveRange(reviews);

            // Xóa showtimes
            _ctx.Showtimes.RemoveRange(showtimes);

            // Xóa movie
            _ctx.Movies.Remove(movie);

            _ctx.SaveChanges();

            TempData["Success"] = "Xóa phim thành công 🗑️";

            return RedirectToAction("Movies");
        }
        public IActionResult Movies()
        {
            var movies = _ctx.Movies.ToList();

            return View(movies);
        }
        [HttpGet]
        public IActionResult Showtimes()
        {
            var showtimes = _ctx.Showtimes
                .Include(x => x.Movie)
                .Include(x => x.Room)
                    .ThenInclude(r => r.Seats)
                .Include(x => x.Tickets)
                .ToList();

            return View(showtimes);
        }
        [HttpGet]
        public IActionResult CreateShowtime()
        {
            ViewBag.Movies = _ctx.Movies.ToList();

            ViewBag.Rooms = _ctx.Rooms.ToList();

            return View();
        }

        [HttpPost]
        public IActionResult CreateShowtime(Showtime showtime)
        {
            _ctx.Showtimes.Add(showtime);

            _ctx.SaveChanges();

            TempData["Success"] = "Add showtime successfully 🎬";

            return RedirectToAction("Showtimes");
        }
        [HttpGet]
        public IActionResult EditShowtime(int id)
        {
            var showtime = _ctx.Showtimes.FirstOrDefault(x => x.Id == id);
            if (showtime == null) return NotFound();

            // Truyền trực tiếp List vào View thay vì ViewBag
            ViewBag.Movies = _ctx.Movies.ToList();
            ViewBag.Rooms = _ctx.Rooms.ToList();

            return View(showtime);
        }

        [HttpPost]
        public IActionResult EditShowtime(Showtime model)
        {
                
            var showtime = _ctx.Showtimes.Find(model.Id);
            if (showtime == null) return NotFound();

            showtime.MovieId = model.MovieId;
            showtime.RoomId = model.RoomId;
            showtime.StartTime = model.StartTime;
            showtime.EndTime = model.EndTime;

            _ctx.SaveChanges();
            TempData["Success"] = "Cập nhật thành công! ✨";
            return RedirectToAction("Showtimes");
        }
        [HttpGet]
        public IActionResult DeleteShowtime(int id)
        {
            var showtime = _ctx.Showtimes.Find(id);

            if (showtime == null)
            {
                return NotFound();
            }

            _ctx.Showtimes.Remove(showtime);

            _ctx.SaveChanges();

            TempData["Success"] = "Delete showtime successfully 🗑️";

            return RedirectToAction("Showtimes");
        }
        public IActionResult Logout()
        {
            // Xóa toàn bộ dữ liệu trong Session (User ID, Role, v.v.)[cite: 1, 2]
            HttpContext.Session.Clear();

            // Quay về trang chủ (Index của HomeController)
            // Cần thêm tham số new { area = "" } để ép trình duyệt thoát ra khỏi vùng Admin
            return RedirectToAction("Index", "Home", new { area = "" });
        }
    }
}