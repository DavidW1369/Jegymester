namespace Jegymester.Entites
{
    public class Screening
    {
        public int Id { get; set; }
        public int RoomId { get; set; }
        public int MovieId { get; set; }
        public DateTime StartTime { get; set; }
        public Movie Movie { get; set; }
        public Room Room { get; set; }
    }
}
