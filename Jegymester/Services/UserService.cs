using Jegymester.Context;
using Jegymester.DTOs;
using Jegymester.Entites;
using Microsoft.EntityFrameworkCore;

namespace Jegymester.Services
{
    public class UserService
    {
        private readonly AppDbContext _db;
        public UserService(AppDbContext db) => _db = db;

        // Filmek és vetítések listázása (Bárki láthatja)
        public List<Screening> GetUpcomingScreenings() =>
            _db.Screenings.Include(s => s.Movie).Where(s => s.StartTime > DateTime.Now).ToList();

        // Jegyvásárlás (Regisztrált és Vendég egyaránt)
        public void PurchaseTickets(TicketOrder order)
        {
            // Megkeressük a felhasználót az email alapján
            var user = _db.Users.FirstOrDefault(u => u.Email == order.Email);

            if (user == null)
            {
                if (string.IsNullOrEmpty(order.PhoneNumber))
                    throw new Exception("Vendégként kötelező megadni a telefonszámot!");
            }
            else
            {
                if (string.IsNullOrEmpty(order.PhoneNumber))
                    order.PhoneNumber = user.PhoneNumber;
            }

            _db.TicketOrders.Add(order);
            _db.SaveChanges();
        }

        public void CancelTicket(int ticketId)
        {
            // Megkeressük a jegyet a vetítési adatokkal együtt
            var ticket = _db.Tickets
                .Include(t => t.Screening)
                .FirstOrDefault(t => t.Id == ticketId);

            if (ticket == null)
                throw new Exception("A jegy nem található!");

            // Ellenőrizzük a 4 órás szabályt (IsCancellable property használata)
            if (!ticket.IsCancellable)
            {
                throw new Exception("A jegy már nem törölhető, mert kevesebb mint 4 óra van a vetítésig!");
            }

            // Ha több jegy volt egy rendelésben (TicketOrder), és ez az utolsó, 
            // akkor magát a rendelést is érdemes lehet törölni vagy kezelni.
            _db.Tickets.Remove(ticket);
            _db.SaveChanges();
        }

        public List<TicketOrder> GetOrdersByEmail(string email)
        {
            return _db.TicketOrders
                .Where(o => o.Email == email)
                .Include(o => o.Tickets)
                    .ThenInclude(t => t.Screening)
                    .ThenInclude(s => s.Movie)
                .OrderByDescending(o => o.PurchaseTime)
                .ToList();
        }

        public void RegisterUser(RegisterDto dto)
        {
            // Ellenőrizzük, hogy létezik-e már ilyen emaillel felhasználó
            if (_db.Users.Any(u => u.Email == dto.Email))
                throw new Exception("Ezzel az e-mail címmel már regisztráltak!");

            var newUser = new User
            {
                Name = dto.Name,
                Email = dto.Email,
                Password = dto.Password, // Éles környezetben itt hashelni kellene!
                PhoneNumber = dto.PhoneNumber,
                Role = Role.RegisteredUser // Alapértelmezett szerepkör
            };

            _db.Users.Add(newUser);
            _db.SaveChanges();
        }
        // UserService.cs
        public User Login(string email, string password)
        {
            // Itt a _db változót használjuk, ami már létezik a Service-ben
            return _db.Users.FirstOrDefault(u => u.Email == email && u.Password == password);
        }

        // UserService.cs
        public List<string> GetBookedSeats(int screeningId)
        {
            // Itt a _db-t használjuk, ami a Service-ben az adatbázisod neve
            return _db.Tickets
                .Where(t => t.ScreeningId == screeningId)
                .Select(t => t.Seat)
                .ToList();
        }
    }
}
