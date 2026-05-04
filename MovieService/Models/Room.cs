using MovieTicket.Models;

namespace MovieService.Models;

public partial class Room
{
    public int Id { get; set; }

    public string? Name { get; set; }

    public int? Capacity { get; set; }

    public int? CinemaId { get; set; }
    public virtual Cinema? Cinema { get; set; }

    public virtual ICollection<Showtime> Showtimes { get; set; }
        = new List<Showtime>();
}