using Microsoft.EntityFrameworkCore;
using BookingService.Models;

namespace BookingService.Data
{
    public class BookingDbContext : DbContext
    {
        public BookingDbContext(
            DbContextOptions<BookingDbContext> options)
            : base(options)
        {
        }

        public DbSet<Order> Orders { get; set; }

        public DbSet<Ticket> Tickets { get; set; }

        public DbSet<Seat> Seats { get; set; }
        public DbSet<ShowtimeSeat> ShowtimeSeats { get; set; }

    }
}