using CinemaApp.Data;
using CinemaApp.Data.Models;
using Microsoft.EntityFrameworkCore;

namespace CinemaApp.Services;

public class FilmService(AppDbContext db)
{
    public Task<List<Film>> GetAllAsync() =>
        db.Films.OrderBy(f => f.Title).ToListAsync();

    public Task<Film?> GetByIdAsync(int id) =>
        db.Films
          .Include(f => f.Screenings)
          .FirstOrDefaultAsync(f => f.Id == id);

    public async Task<Film> CreateAsync(Film film)
    {
        db.Films.Add(film);
        await db.SaveChangesAsync();
        return film;
    }

    public async Task UpdateAsync(Film film)
    {
        db.Films.Update(film);
        await db.SaveChangesAsync();
    }

    /// <summary>
    /// Deletes a film. Returns false if it has any future screenings.
    /// </summary>
    public async Task<bool> DeleteAsync(int id)
    {
        var hasFutureScreenings = await db.Screenings
            .AnyAsync(s => s.FilmId == id && s.StartTime > DateTime.UtcNow);

        if (hasFutureScreenings) return false;

        var film = await db.Films.FindAsync(id);
        if (film is null) return false;

        db.Films.Remove(film);
        await db.SaveChangesAsync();
        return true;
    }
}
