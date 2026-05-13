using CinemaApp.Data.Models;
using Microsoft.AspNetCore.Identity;

namespace CinemaApp.Data;

/// <summary>
/// Populates the database with realistic example data for development and testing.
/// Only called on a fresh database (when cinema.db is first created).
/// To re-run: delete cinema.db and restart the app.
/// </summary>
public static class SeedTestData
{
    // Rooms are already seeded via EF HasData in AppDbContext:
    //   Room 1 — Room A (Small,  50 seats)
    //   Room 2 — Room B (Medium, 100 seats)
    //   Room 3 — Room C (Large,  200 seats)

    public static async Task PopulateAsync(AppDbContext db, UserManager<AppUser> userManager)
    {
        // ── Films ────────────────────────────────────────────────────────────
        var films = new List<Film>
        {
            new() {
                Title           = "Stellar Drift",
                DurationMinutes = 132,
                Description     = "A lone astronaut stranded beyond Neptune must improvise her way home using nothing but a broken probe and sheer willpower. Stunning visuals, intimate storytelling."
            },
            new() {
                Title           = "The Hollow Crown",
                DurationMinutes = 148,
                Description     = "A medieval epic following a disgraced knight who uncovers a conspiracy reaching the very throne. Based on the bestselling novel."
            },
            new() {
                Title           = "Neon Ghosts",
                DurationMinutes = 98,
                Description     = "A neo-noir thriller set in a rain-soaked near-future city where a private detective discovers her missing client was never real."
            },
            new() {
                Title           = "Last Harvest",
                DurationMinutes = 112,
                Description     = "A slow-burn horror about a farming family whose remote land starts producing something it shouldn't. Deeply unsettling."
            },
            new() {
                Title           = "Doppio",
                DurationMinutes = 89,
                Description     = "A light Italian comedy about twin brothers who accidentally swap lives during a family wedding and can't figure out how to swap back."
            },
        };

        db.Films.AddRange(films);
        await db.SaveChangesAsync();

        // ── Screenings ───────────────────────────────────────────────────────
        // Mix of past (for testing order history) and future (for buying tickets).
        var now = DateTime.UtcNow;

        var screenings = new List<Screening>
        {
            // Past screenings — useful for confirming ticket display in MyTickets
            new() { FilmId = films[0].Id, RoomId = 1, StartTime = now.AddDays(-5).AddHours(14) },
            new() { FilmId = films[1].Id, RoomId = 2, StartTime = now.AddDays(-3).AddHours(19) },
            new() { FilmId = films[2].Id, RoomId = 3, StartTime = now.AddDays(-1).AddHours(20) },

            // Upcoming screenings — spread across films and rooms
            new() { FilmId = films[0].Id, RoomId = 1, StartTime = now.AddDays(1).AddHours(15)  },
            new() { FilmId = films[0].Id, RoomId = 2, StartTime = now.AddDays(1).AddHours(19)  },
            new() { FilmId = films[1].Id, RoomId = 3, StartTime = now.AddDays(2).AddHours(18)  },
            new() { FilmId = films[2].Id, RoomId = 1, StartTime = now.AddDays(2).AddHours(21)  },
            new() { FilmId = films[3].Id, RoomId = 2, StartTime = now.AddDays(3).AddHours(20)  },
            new() { FilmId = films[4].Id, RoomId = 3, StartTime = now.AddDays(3).AddHours(17)  },
            new() { FilmId = films[1].Id, RoomId = 1, StartTime = now.AddDays(5).AddHours(16)  },
            new() { FilmId = films[3].Id, RoomId = 3, StartTime = now.AddDays(6).AddHours(19)  },
            new() { FilmId = films[4].Id, RoomId = 2, StartTime = now.AddDays(7).AddHours(14)  },
        };

        db.Screenings.AddRange(screenings);
        await db.SaveChangesAsync();

        // ── Test accounts ────────────────────────────────────────────────────
        var testUser    = await CreateUserAsync(userManager, "user@cinema.com",    "+1-555-010-0001", Role.User,    "User123!");
        var testCashier = await CreateUserAsync(userManager, "cashier@cinema.com", "+1-555-010-0002", Role.Cashier, "Cashier123!");

        // ── Pre-bought orders ────────────────────────────────────────────────
        // Gives the test user a realistic order history to browse in My Tickets.

        // Past order — two tickets, both on a past screening (already over)
        db.TicketOrders.Add(new TicketOrder
        {
            UserId    = testUser.Id,
            OrderDate = now.AddDays(-5).AddHours(-2),
            Tickets   =
            [
                new Ticket { ScreeningId = screenings[0].Id, SeatNumber = 12, IsConfirmed = true  },
                new Ticket { ScreeningId = screenings[0].Id, SeatNumber = 13, IsConfirmed = true  },
            ]
        });

        // Another past order — single ticket, not confirmed (cashier forgot)
        db.TicketOrders.Add(new TicketOrder
        {
            UserId    = testUser.Id,
            OrderDate = now.AddDays(-3).AddHours(-1),
            Tickets   =
            [
                new Ticket { ScreeningId = screenings[1].Id, SeatNumber = 5, IsConfirmed = false },
            ]
        });

        // Future order — still cancellable (more than 4 hours away)
        db.TicketOrders.Add(new TicketOrder
        {
            UserId    = testUser.Id,
            OrderDate = now.AddHours(-1),
            Tickets   =
            [
                new Ticket { ScreeningId = screenings[3].Id, SeatNumber = 8,  IsConfirmed = false },
                new Ticket { ScreeningId = screenings[3].Id, SeatNumber = 9,  IsConfirmed = false },
                new Ticket { ScreeningId = screenings[3].Id, SeatNumber = 10, IsConfirmed = false },
            ]
        });

        // Guest order on an upcoming screening
        db.TicketOrders.Add(new TicketOrder
        {
            GuestEmail = "alice@example.com",
            GuestPhone = "+1-555-999-1234",
            OrderDate  = now.AddHours(-3),
            Tickets    =
            [
                new Ticket { ScreeningId = screenings[5].Id, SeatNumber = 42, IsConfirmed = false },
            ]
        });

        // Cashier in-person order (no linked customer — uses cinema contact info)
        db.TicketOrders.Add(new TicketOrder
        {
            GuestEmail = CinemaConstants.CinemaEmail,
            GuestPhone = CinemaConstants.CinemaPhone,
            OrderDate  = now.AddHours(-2),
            Tickets    =
            [
                new Ticket { ScreeningId = screenings[7].Id, SeatNumber = 1, IsConfirmed = false },
                new Ticket { ScreeningId = screenings[7].Id, SeatNumber = 2, IsConfirmed = false },
            ]
        });

        // Partially fill a future screening to make the seat grid interesting
        var bulkSeats = Enumerable.Range(20, 18)   // seats 20–37
            .Select(s => new Ticket
            {
                ScreeningId = screenings[4].Id,
                SeatNumber  = s,
                IsConfirmed = false
            })
            .ToList();

        db.TicketOrders.Add(new TicketOrder
        {
            GuestEmail = "groupbooking@example.com",
            GuestPhone = "+1-555-000-5555",
            OrderDate  = now.AddDays(-1),
            Tickets    = bulkSeats
        });

        await db.SaveChangesAsync();
    }

    private static async Task<AppUser> CreateUserAsync(
        UserManager<AppUser> userManager,
        string email, string phone, Role role, string password)
    {
        var roleName = role.ToString();
        var user = new AppUser
        {
            UserName    = email,
            Email       = email,
            PhoneNumber = phone,
            Role        = role
        };
        var result = await userManager.CreateAsync(user, password);
        if (result.Succeeded)
            await userManager.AddToRoleAsync(user, roleName);

        return user;
    }
}
