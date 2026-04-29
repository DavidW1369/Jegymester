using Jegymester.Context;
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
            var screening = _db.Screenings.Include(s => s.Room).FirstOrDefault(s => s.Id == order.Tickets.First().ScreeningId);

            // Szabad helyek ellenőrzése
            int bookedSeats = _db.Tickets.Count(t => t.ScreeningId == screening.Id);
            if (bookedSeats + order.Tickets.Count > screening.Room.Capacity)
                throw new Exception("Nincs elég szabad hely a teremben!");

            // Vendég adatok ellenőrzése (ha nincs UserId)
            if (order.UserId == null && (string.IsNullOrEmpty(order.Email) || string.IsNullOrEmpty(order.PhoneNumber)))
                throw new Exception("Vendégként kötelező megadni az e-mailt és telefonszámot!");

            order.PurchaseTime = DateTime.Now;
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

        public List<TicketOrder> GetUserOrders(int userId)
        {
            return _db.TicketOrders
                .Where(o => o.UserId == userId)
                .Include(o => o.Tickets)
                    .ThenInclude(t => t.Screening)
                    .ThenInclude(s => s.Movie)
                .OrderByDescending(o => o.PurchaseTime)
                .ToList();
        }


    }
}
