using System;
using System.Collections.Generic;

namespace MovieTicket.Models;

public partial class Cinema
{
    public int Id { get; set; }

    public string? Name { get; set; }

    public string? Address { get; set; }

    public string? City { get; set; }

    public string? Hotline { get; set; }

    public string? Status { get; set; }

    public virtual ICollection<Room> Rooms { get; set; } = new List<Room>();
}
