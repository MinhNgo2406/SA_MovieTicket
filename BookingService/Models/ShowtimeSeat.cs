using BookingService.Models;

namespace BookingService.Models;

public partial class ShowtimeSeat
{
    public int Id { get; set; }

    public int? ShowtimeId { get; set; }

    public int? SeatId { get; set; }

    public string? Status { get; set; }

    public virtual Seat? Seat { get; set; }
}
