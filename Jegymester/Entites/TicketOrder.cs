namespace Jegymester.Entites
{
    public class TicketOrder
    {
        public int Id { get;set; }
        public List<Ticket> Tickets { get; set; }
        public DateTime PurchaseTime { get; set; }
        public string PhoneNumber { get; set; }
        public string Email { get; set; }
    }
}
