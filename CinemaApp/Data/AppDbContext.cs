using CinemaApp.Data.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace CinemaApp.Data;

public class AppDbContext(DbContextOptions<AppDbContext> options) : IdentityDbContext<AppUser>(options)
{
    public DbSet<Film> Films => Set<Film>();
    public DbSet<Room> Rooms => Set<Room>();
    public DbSet<Screening> Screenings => Set<Screening>();
    public DbSet<Ticket> Tickets => Set<Ticket>();
    public DbSet<TicketOrder> TicketOrders => Set<TicketOrder>();

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);

        // No two tickets may claim the same seat in the same screening.
        builder.Entity<Ticket>()
            .HasIndex(t => new { t.ScreeningId, t.SeatNumber })
            .IsUnique();

        // TicketOrder.UserId is the only FK to AppUser now.
        builder.Entity<TicketOrder>()
            .HasOne(o => o.User)
            .WithMany()
            .HasForeignKey(o => o.UserId)
            .OnDelete(DeleteBehavior.Restrict);

        // Three fixed room sizes seeded at migration time.
        builder.Entity<Room>().HasData(
            new Room { Id = 1, Name = "Room A (Small)",  SeatsNum = 50  },
            new Room { Id = 2, Name = "Room B (Medium)", SeatsNum = 100 },
            new Room { Id = 3, Name = "Room C (Large)",  SeatsNum = 200 }
        );
    }
}
