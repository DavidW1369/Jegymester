using Jegymester.Entites;
using Microsoft.EntityFrameworkCore;

namespace Jegymester.Context
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions options) : base(options)
        {
        }

        public DbSet<User> Users { get; set; }
        public DbSet<Room> Rooms { get; set; }
        public DbSet<Movie> Movies { get; set; }
        public DbSet<Screening> Screenings { get; set; }
        public DbSet<Ticket> Tickets { get; set; }
        public DbSet<TicketOrder> TicketOrders { get; set; }
        public DbSet<Transaction> Transactions { get; set; }


    }
}
