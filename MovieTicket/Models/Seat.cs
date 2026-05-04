using System;
using System.Collections.Generic;

namespace MovieTicket.Models;

public partial class Seat
{
    public int Id { get; set; }

    public int? RoomId { get; set; }

    public string? RowName { get; set; }

    public int? SeatNumber { get; set; }

    public virtual Room? Room { get; set; }

    public virtual ICollection<ShowtimeSeat> ShowtimeSeats { get; set; } = new List<ShowtimeSeat>();

    public virtual ICollection<Ticket> Tickets { get; set; } = new List<Ticket>();
}
