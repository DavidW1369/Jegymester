namespace Jegymester.Entites
{
    public class TicketOrder
    {
        public int Id { get;set; }
        public int UserId { get; set; }
        public List<Ticket> Tickets { get; set; }
        public User User { get; set; }
    }
}
