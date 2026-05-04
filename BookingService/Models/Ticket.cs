namespace BookingService.Models;

public partial class Ticket
{
    public int Id { get; set; }

    public int OrderId { get; set; }

    public int ShowTimeId { get; set; }

    public int SeatId { get; set; }

    public decimal Price { get; set; }

    public string? Status { get; set; }

    public DateTime? CreatedAt { get; set; }

    public virtual Order Order { get; set; } = null!;

    public virtual Seat Seat { get; set; } = null!;

}
