using Jegymester.Context;
using Jegymester.Entites;
using Microsoft.EntityFrameworkCore;

namespace Jegymester.Services
{
    public class CashierService
    {
        private readonly AppDbContext _db;
        public CashierService(AppDbContext db) => _db = db;

        // Jegy érvényesítése (pl. beléptetéskor)
        public string ValidateTicket(int ticketId)
        {
            var ticket = _db.Tickets
                .Include(t => t.Screening)
                .ThenInclude(s => s.Movie)
                .FirstOrDefault(t => t.Id == ticketId);

            if (ticket == null) return "A jegy nem létezik!";

            if (ticket.IsUsed) return "Ezt a jegyet már felhasználták!";

            // Időellenőrzés (amit te írtál)
            bool isTimeValid = ticket.Screening.StartTime > DateTime.Now.AddHours(-3)
                               && ticket.Screening.StartTime < DateTime.Now.AddHours(1);
            // Plusz infó: ne engedjük be 1 órával a kezdés előttnél hamarabb

            if (!isTimeValid) return "A jegy nem erre az időpontra szól, vagy már lejárt!";

            // Ha minden oké, érvényesítjük
            ticket.IsUsed = true;
            _db.SaveChanges();

            return $"OK: Belépés engedélyezve! ({ticket.Screening.Movie.Name}, Szék: {ticket.Seat})";
        }

        public void ProcessCashierPurchase(TicketOrder order)
        {
            // Itt jöhetnek extra ellenőrzések (pl. fizetési mód: készpénz/kártya naplózása)
            _db.TicketOrders.Add(order);
            _db.SaveChanges();
        }
    }
}
