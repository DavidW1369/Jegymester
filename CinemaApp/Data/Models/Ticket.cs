namespace CinemaApp.Data.Models;

public class Ticket
{
    public int Id { get; set; }

    public int ScreeningId { get; set; }
    public Screening Screening { get; set; } = null!;

    // Seat number within the room (1 to Room.SeatsNum).
    // Unique per screening enforced by DB index.
    public int SeatNumber { get; set; }

    // Flipped by a cashier after physical verification at the door.
    public bool IsConfirmed { get; set; }

    public int TicketOrderId { get; set; }
    public TicketOrder TicketOrder { get; set; } = null!;
}
