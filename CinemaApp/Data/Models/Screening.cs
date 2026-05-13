namespace CinemaApp.Data.Models;

public class Screening
{
    public int Id { get; set; }

    public int FilmId { get; set; }
    public Film Film { get; set; } = null!;

    public int RoomId { get; set; }
    public Room Room { get; set; } = null!;

    public DateTime StartTime { get; set; }

    public ICollection<Ticket> Tickets { get; set; } = [];
}
