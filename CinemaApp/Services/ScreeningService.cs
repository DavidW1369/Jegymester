using CinemaApp.Data;
using CinemaApp.Data.Models;
using Microsoft.EntityFrameworkCore;

namespace CinemaApp.Services;

public class ScreeningService(AppDbContext db)
{
    public Task<List<Screening>> GetAllAsync() =>
        db.Screenings
          .Include(s => s.Film)
          .Include(s => s.Room)
          .Include(s => s.Tickets)
          .OrderBy(s => s.StartTime)
          .ToListAsync();

    /// <summary>Returns upcoming screenings for a specific film, including ticket counts.</summary>
    public Task<List<Screening>> GetUpcomingByFilmAsync(int filmId) =>
        db.Screenings
          .Include(s => s.Room)
          .Include(s => s.Tickets)
          .Where(s => s.FilmId == filmId && s.StartTime > DateTime.UtcNow)
          .OrderBy(s => s.StartTime)
          .ToListAsync();

    public Task<Screening?> GetByIdAsync(int id) =>
        db.Screenings
          .Include(s => s.Film)
          .Include(s => s.Room)
          .Include(s => s.Tickets)
          .FirstOrDefaultAsync(s => s.Id == id);

    public Task<List<Room>> GetRoomsAsync() =>
        db.Rooms.OrderBy(r => r.SeatsNum).ToListAsync();

    public async Task<Screening> CreateAsync(Screening screening)
    {
        db.Screenings.Add(screening);
        await db.SaveChangesAsync();
        return screening;
    }

    public async Task UpdateAsync(Screening screening)
    {
        db.Screenings.Update(screening);
        await db.SaveChangesAsync();
    }

    /// <summary>
    /// Deletes a screening. Returns false if any tickets have been sold.
    /// </summary>
    public async Task<bool> DeleteAsync(int id)
    {
        var screening = await db.Screenings
            .Include(s => s.Tickets)
            .FirstOrDefaultAsync(s => s.Id == id);

        if (screening is null)          return false;
        if (screening.Tickets.Count > 0) return false;

        db.Screenings.Remove(screening);
        await db.SaveChangesAsync();
        return true;
    }
}
