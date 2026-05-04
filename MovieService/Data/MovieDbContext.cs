using Microsoft.EntityFrameworkCore;
using MovieService.Models;
using MovieTicket.Models;

namespace MovieService.Data
{
    public class MovieDbContext : DbContext
    {
        public MovieDbContext(DbContextOptions<MovieDbContext> options)
            : base(options)
        {
        }

        public DbSet<Movie> Movies { get; set; }
        public DbSet<Showtime> Showtimes { get; set; }
        public DbSet<Room> Rooms { get; set; }
        public DbSet<Cinema> Cinemas { get; set; }
        public DbSet<Review> Reviews { get; set; }

        // THÊM ĐOẠN NÀY ĐỂ MAPPING CHÍNH XÁC TÊN BẢNG TRONG DATABASE
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // Ép Entity Framework map model Movie vào bảng "Movies" (số nhiều)
            modelBuilder.Entity<Movie>().ToTable("Movies");

            // Nếu sau này các bảng khác (Cinema, Room...) báo lỗi tương tự, 
            // bạn chỉ cần thêm dòng tương tự cho bảng đó. 
            // Ví dụ: modelBuilder.Entity<Cinema>().ToTable("Cinema");
            
            base.OnModelCreating(modelBuilder);
        }
    }
}