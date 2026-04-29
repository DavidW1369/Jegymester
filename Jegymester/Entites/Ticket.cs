namespace Jegymester.Entites
{
    public class Ticket
    {
        public int Id { get; set; }
        public int ScreeningId { get; set; }
        public string Seat { get; set; }
        public bool IsCancellable => (Screening.StartTime - DateTime.Now).TotalHours >= 4;
        public int TicketOrderId { get; set; }

        public Screening Screening { get; set; }
    }
}
