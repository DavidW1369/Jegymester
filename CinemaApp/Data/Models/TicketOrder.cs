namespace CinemaApp.Data.Models;

public class TicketOrder
{
    public int Id { get; set; }
    public DateTime OrderDate { get; set; }

    // Set when a registered user buys for themselves online,
    // or when a cashier links the order to a known customer by email.
    // Null for anonymous guest purchases.
    public string? UserId { get; set; }
    public AppUser? User { get; set; }

    // For guest purchases: provided by the guest at checkout.
    // For cashier in-person with no known customer: CinemaConstants email/phone.
    // Null when UserId is set (contact info lives on AppUser).
    public string? GuestEmail { get; set; }
    public string? GuestPhone { get; set; }

    public ICollection<Ticket> Tickets { get; set; } = [];
}
