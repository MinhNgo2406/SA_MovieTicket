using Microsoft.AspNetCore.Mvc;
using MovieTicket.Models;

namespace MovieTicket.Controllers
{
    public class MovieController : Controller
    { 
        public MovieBookingContext ctx;
        public MovieController(MovieBookingContext ctx)
        {
            this.ctx = ctx;
        }
        public IActionResult Details(int id)
        {
            var movie = ctx.Movies.FirstOrDefault(x => x.Id == id);

            var showtimes = ctx.Showtimes
                .Where(x => x.MovieId == id)
                .Select(x => new
                {
                    x.Id,
                    x.StartTime,
                    CinemaName = x.Room.Cinema.Name
                }) 
                .OrderBy(x => x.StartTime) // thêm cái này cho chuẩn
                .ToList();

            ViewBag.Showtimes = showtimes;

            // 👉 THÊM DÒNG NÀY (QUAN TRỌNG)
            ViewBag.FirstShowtimeId = showtimes.FirstOrDefault()?.Id;

            ViewBag.NowShowing = ctx.Movies
                .Where(x => x.Id != id)
                .Take(6)
                .ToList();

            return View(movie);
        }
    }
}
