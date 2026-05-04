using MovieTicket.Models;
namespace MovieService.Models;

public partial class Movie
{
    public int Id { get; set; }

    public string? Title { get; set; }

    public string? Description { get; set; }

    public string? Image { get; set; }
    public DateTime? ReleaseDate { get; set; }
    public int Duration { get; set; }

    public double Rating { get; set; }
   
    public virtual ICollection<Review> Reviews { get; set; } = new List<Review>();
    public virtual ICollection<Showtime> Showtimes { get; set; }
        = new List<Showtime>();
}