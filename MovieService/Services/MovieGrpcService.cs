using Grpc.Core;
using Microsoft.EntityFrameworkCore;
using MovieService.Data;

namespace MovieService.Services
{
    public class MovieGrpcService : MovieGrpc.MovieGrpcBase
    {
        private readonly MovieDbContext _ctx;

        public MovieGrpcService(MovieDbContext ctx)
        {
            _ctx = ctx;
        }

        // Unary RPC
        public override Task<MovieReply> GetMovieById(
            MovieRequest request,
            ServerCallContext context)
        {
            var movie = _ctx.Movies.Find(request.Id);
            if (movie == null) throw new RpcException(new Status(StatusCode.NotFound, "Phim không tồn tại"));
            return Task.FromResult(new MovieReply
            {
                Id = movie.Id,
                Title = movie.Title ?? "",
                Image = movie.Image ?? "",
                Rating = movie.Rating,
                Description = movie.Description ?? "",
                Duration = movie.Duration
            });
        }

        // Streaming RPC
        public override async Task GetAllMovies(
            Empty request,
            IServerStreamWriter<MovieReply> responseStream,
            ServerCallContext context)
        {
            var movies = _ctx.Movies.ToList();

            foreach (var movie in movies)
            {
                await responseStream.WriteAsync(new MovieReply
                {
                    Id = movie.Id,
                    Title = movie.Title ?? "",
                    Image = movie.Image ?? "",
                    Rating = movie.Rating,
                    Description = movie.Description ?? "",
                    Duration = movie.Duration
                });
            }
        }
        public override async Task GetCinemas(Empty request, IServerStreamWriter<CinemaReply> responseStream, ServerCallContext context)
        {
            var cinemas = await _ctx.Cinemas.ToListAsync();
            foreach (var cinema in cinemas)
            {
                await responseStream.WriteAsync(new CinemaReply
                {
                    Id = cinema.Id,
                    Name = cinema.Name ?? "",
                    Address = cinema.Address ?? "",
                    City = cinema.City ?? ""
                });
            }
        }
        public override async Task GetShowtimesByMovie(MovieRequest request, IServerStreamWriter<ShowtimeReply> responseStream, ServerCallContext context)
        {
            var showtimes = await _ctx.Showtimes
                .Include(s => s.Room)
                .ThenInclude(r => r.Cinema)
                .Where(s => s.MovieId == request.Id)
                .ToListAsync();

            foreach (var s in showtimes)
            {
                await responseStream.WriteAsync(new ShowtimeReply
                {
                    Id = s.Id,
                    StartTime = s.StartTime?.ToString("yyyy-MM-dd HH:mm") ?? "",
                    RoomName = s.Room?.Name ?? "",
                    CinemaName = s.Room?.Cinema?.Name ?? ""
                });
            }
        }
        public override async Task GetReviewsByMovie(MovieRequest request, IServerStreamWriter<ReviewReply> responseStream, ServerCallContext context)
        {
            var reviews = await _ctx.Reviews
                .Where(r => r.MovieId == request.Id)
                .ToListAsync();

            foreach (var r in reviews)
            {
                await responseStream.WriteAsync(new ReviewReply
                {
                    Id = r.Id,
                    Comment = r.Comment ?? "",
                    Rating = r.Rating ?? 0,
                    CreatedAt = r.CreatedAt?.ToString("dd/MM/yyyy") ?? "",
                    UserId = r.UserId ?? 0
                });
            }
        }
    }
}