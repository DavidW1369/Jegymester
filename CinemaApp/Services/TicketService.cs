using CinemaApp.Data;
using CinemaApp.Data.Models;
using Microsoft.EntityFrameworkCore;

namespace CinemaApp.Services;

public class TicketService(AppDbContext db)
{
    public Task<List<int>> GetTakenSeatsAsync(int screeningId) =>
        db.Tickets
          .Where(t => t.ScreeningId == screeningId)
          .Select(t => t.SeatNumber)
          .ToListAsync();

    /// <summary>
    /// Creates a TicketOrder with one Ticket per selected seat.
    ///
    /// Supply userId for a registered user (self-purchase or cashier-linked customer).
    /// Leave userId null for anonymous guests — guestEmail and guestPhone are then required,
    /// unless this is a cashier walk-in with no customer details (caller passes cinema constants).
    /// </summary>
    public async Task<(bool Success, string? Error)> BuyTicketsAsync(
        int screeningId,
        List<int> seatNumbers,
        string? userId,
        string? guestEmail,
        string? guestPhone)
    {
        if (seatNumbers.Count == 0)
            return (false, "Select at least one seat.");

        var screening = await db.Screenings
            .Include(s => s.Room)
            .FirstOrDefaultAsync(s => s.Id == screeningId);

        if (screening is null)
            return (false, "Screening not found.");

        if (screening.StartTime <= DateTime.UtcNow)
            return (false, "This screening has already started.");

        var invalid = seatNumbers.Where(s => s < 1 || s > screening.Room.SeatsNum).ToList();
        if (invalid.Count > 0)
            return (false, $"Seat(s) {string.Join(", ", invalid)} are out of range for this room.");

        var taken = await GetTakenSeatsAsync(screeningId);
        var conflicts = seatNumbers.Intersect(taken).ToList();
        if (conflicts.Count > 0)
            return (false, $"Seat(s) {string.Join(", ", conflicts)} are already taken.");

        // Anonymous guest must provide contact info (cinema constants count as provided).
        if (userId is null && string.IsNullOrWhiteSpace(guestEmail))
            return (false, "Email and phone number are required for guest purchases.");

        var order = new TicketOrder
        {
            OrderDate  = DateTime.UtcNow,
            UserId     = userId,
            GuestEmail = userId is null ? guestEmail : null,
            GuestPhone = userId is null ? guestPhone : null,
            Tickets    = seatNumbers
                .Select(seat => new Ticket
                {
                    ScreeningId = screeningId,
                    SeatNumber  = seat,
                    IsConfirmed = false
                })
                .ToList()
        };

        db.TicketOrders.Add(order);

        try
        {
            await db.SaveChangesAsync();
            return (true, null);
        }
        catch (DbUpdateException)
        {
            // Unique index violation — another request claimed a seat simultaneously.
            return (false, "A seat was just taken. Please refresh and try again.");
        }
    }

    /// <summary>
    /// Cancels an order. Only the order's owner may cancel, and only more than
    /// 4 hours before the earliest screening in the order.
    /// </summary>
    public async Task<(bool Success, string? Error)> CancelOrderAsync(int orderId, string requestingUserId)
    {
        var order = await db.TicketOrders
            .Include(o => o.Tickets)
                .ThenInclude(t => t.Screening)
            .FirstOrDefaultAsync(o => o.Id == orderId);

        if (order is null)               return (false, "Order not found.");
        if (order.UserId != requestingUserId) return (false, "Not authorised to cancel this order.");

        var earliestStart = order.Tickets.Min(t => t.Screening.StartTime);
        if (DateTime.UtcNow >= earliestStart - TimeSpan.FromHours(4))
            return (false, "Orders can only be cancelled more than 4 hours before the screening.");

        db.Tickets.RemoveRange(order.Tickets);
        db.TicketOrders.Remove(order);
        await db.SaveChangesAsync();
        return (true, null);
    }

    /// <summary>Confirms a ticket (cashier marks it as used at the door).</summary>
    public async Task<(bool Success, string? Error)> ConfirmTicketAsync(int ticketId)
    {
        var ticket = await db.Tickets
            .Include(t => t.Screening)
                .ThenInclude(s => s.Film)
            .Include(t => t.Screening)
                .ThenInclude(s => s.Room)
            .Include(t => t.TicketOrder)
            .FirstOrDefaultAsync(t => t.Id == ticketId);

        if (ticket is null)            return (false, "Ticket not found.");
        if (ticket.IsConfirmed)        return (false, "Ticket is already confirmed.");

        ticket.IsConfirmed = true;
        await db.SaveChangesAsync();
        return (true, null);
    }

    public Task<Ticket?> GetTicketByIdAsync(int ticketId) =>
        db.Tickets
          .Include(t => t.Screening).ThenInclude(s => s.Film)
          .Include(t => t.Screening).ThenInclude(s => s.Room)
          .Include(t => t.TicketOrder)
          .FirstOrDefaultAsync(t => t.Id == ticketId);

    public Task<List<TicketOrder>> GetOrdersForUserAsync(string userId) =>
        db.TicketOrders
          .Include(o => o.Tickets).ThenInclude(t => t.Screening).ThenInclude(s => s.Film)
          .Include(o => o.Tickets).ThenInclude(t => t.Screening).ThenInclude(s => s.Room)
          .Where(o => o.UserId == userId)
          .OrderByDescending(o => o.OrderDate)
          .ToListAsync();
}
