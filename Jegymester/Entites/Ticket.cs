namespace Jegymester.Entites
{
    public class Ticket
    {
        public int Id { get; set; }
        public int ScreeningId { get; set; }
        public string Seat { get; set; }
        public bool Cancellable { get; set; }
        public int TicketOrderId { get; set; }

    }
}
