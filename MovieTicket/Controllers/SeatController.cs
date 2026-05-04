using Microsoft.AspNetCore.Mvc;
using MovieTicket.Models;

namespace MovieTicket.Controllers
{
    public class SeatController : Controller
    {
        public MovieBookingContext ctx;
        public SeatController(MovieBookingContext ctx)
        {
            this.ctx = ctx;
        }
        public IActionResult SeatSelection(int showtimeId)
        {
            var username = HttpContext.Session.GetString("Username");

            if (username == null)
            {
                return RedirectToAction("Login", "Account");
            }
            var showtime = ctx.Showtimes
                .Where(x => x.Id == showtimeId)
                .Select(x => new
                {
                    x.Id,
                    x.StartTime,
                    x.RoomId,
                    x.MovieId,
                    MovieTitle = x.Movie.Title,
                    x.Movie.Duration,
                    CinemaName = x.Room.Cinema.Name
                })
                .FirstOrDefault();

            if (showtime == null)
                return NotFound();

            // 🔹 seats
            var seats = ctx.Seats
                .Where(s => s.RoomId == showtime.RoomId)
                .Select(s => new
                {
                    s.Id,
                    s.RowName,
                    s.SeatNumber
                })
                .ToList();

            // 🔹 booked seats
            var bookedSeatIds = ctx.Tickets
                .Where(t => t.ShowTimeId == showtimeId)
                .Select(t => t.SeatId)
                .ToList();

            // 🔹 showtimes (FIX LỖI)
            var showtimes = ctx.Showtimes
                .Where(x => x.MovieId == showtime.MovieId)
                .Select(x => new
                {
                    x.Id,
                    x.StartTime
                })
                .ToList();

            ViewBag.Showtime = showtime;
            ViewBag.Seats = seats;
            ViewBag.BookedSeats = bookedSeatIds;
            ViewBag.Showtimes = showtimes;

            return View();
        }
        [HttpPost]
        public IActionResult CreateBooking(int showtimeId,string selectedSeatIds)
        {
            try
            {
                var username =
                    HttpContext.Session.GetString("Username");

                var user = ctx.Users
                    .FirstOrDefault(x => x.Name == username);

                if (user == null)
                    return Content("User null");

                // convert ids
                var seatIds = selectedSeatIds
                    .Split(',')
                    .Select(int.Parse)
                    .ToList();

                decimal total =
                    seatIds.Count * 100000;

                // create order
                var order = new Order
                {
                    UserId = user.Id,
                    TotalAmount = total,
                    Status = "Pending",
                    CreatedAt = DateTime.Now
                };

                ctx.Orders.Add(order);

                ctx.SaveChanges();

                // create ticket
                foreach (var seatId in seatIds)
                {
                    var ticket = new Ticket
                    {
                        OrderId = order.Id,
                        SeatId = seatId,
                        ShowTimeId = showtimeId,
                        Price = 100000
                    };

                    ctx.Tickets.Add(ticket);
                }

                ctx.SaveChanges();

                return RedirectToAction(
                    "Checkout",
                    "Payment",
                    new { orderId = order.Id });
            }
            catch (Exception ex)
            {
                return Content(ex.Message);
            }
        }
    }
}
