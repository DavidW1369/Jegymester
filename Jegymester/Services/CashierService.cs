using Jegymester.Context;
using Microsoft.EntityFrameworkCore;

namespace Jegymester.Services
{
    public class CashierService
    {
        private readonly AppDbContext _db;
        public CashierService(AppDbContext db) => _db = db;

        // Jegy érvényesítése (pl. beléptetéskor)
        public bool ValidateTicket(int ticketId)
        {
            var ticket = _db.Tickets.Include(t => t.Screening).FirstOrDefault(t => t.Id == ticketId);
            if (ticket == null) return false;

            // Csak akkor érvényes, ha a vetítés még nem múlt el
            return ticket.Screening.StartTime > DateTime.Now.AddHours(-3);
        }

        // A pénztáros ugyanazt a PurchaseTickets-et hívhatja meg a UserService-ből, 
        // de ő adja meg a fizikai vásárló adatait.
    }
}
