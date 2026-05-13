namespace CinemaApp.Data.Models;

public class Film
{
    public int Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public int DurationMinutes { get; set; }

    public ICollection<Screening> Screenings { get; set; } = [];
}
