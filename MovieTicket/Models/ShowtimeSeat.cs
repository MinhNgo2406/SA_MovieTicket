using System;
using System.Collections.Generic;

namespace MovieTicket.Models;

public partial class ShowtimeSeat
{
    public int Id { get; set; }

    public int? ShowtimeId { get; set; }

    public int? SeatId { get; set; }

    public string? Status { get; set; }

    public virtual Seat? Seat { get; set; }

    public virtual Showtime? Showtime { get; set; }
}
