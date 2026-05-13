namespace CinemaApp.Data.Models;

public class Room
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;

    // Total number of seats. Tickets reference a seat by number (1–SeatsNum).
    public int SeatsNum { get; set; }

    public ICollection<Screening> Screenings { get; set; } = [];
}
