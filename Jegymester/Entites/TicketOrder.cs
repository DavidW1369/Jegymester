namespace Jegymester.Entites
{
    public class TicketOrder
    {
        public int Id { get;set; }
        public int? UserId { get; set; }
        public List<Ticket> Tickets { get; set; }
        public User User { get; set; }
        public DateTime PurchaseTime { get; set; }
        public string PhoneNumber { get; set; }
        public string Email { get; set; }
    }
}
