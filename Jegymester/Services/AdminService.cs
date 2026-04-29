using Jegymester.Context;
using Jegymester.Entites;

namespace Jegymester.Services
{
    public class AdminService
    {
        private readonly AppDbContext _db;
        public AdminService(AppDbContext db) => _db = db;

        // Új film felvitele
        public void AddMovie(Movie movie)
        {
            _db.Movies.Add(movie);
            _db.SaveChanges();
        }

        // Film törlése (Csak ha nincs aktív vetítése)
        public void DeleteMovie(int movieId)
        {
            bool hasActiveScreening = _db.Screenings.Any(s => s.MovieId == movieId && s.StartTime > DateTime.Now);
            if (hasActiveScreening)
                throw new Exception("A film nem törölhető, mert van hozzá aktív vetítés!");

            var movie = _db.Movies.Find(movieId);
            if (movie != null)
            {
                _db.Movies.Remove(movie);
                _db.SaveChanges();
            }
        }

        // Új vetítés létrehozása
        public void CreateScreening(Screening screening)
        {
            // Ütközés figyelés: ugyanabban a teremben, ugyanakkor ne legyen másik film
            bool overlap = _db.Screenings.Any(s => s.RoomId == screening.RoomId &&
                                                  s.StartTime < screening.StartTime.AddHours(3) &&
                                                  s.StartTime > screening.StartTime.AddHours(-3));
            if (overlap) throw new Exception("A terem ebben az időpontban foglalt!");

            _db.Screenings.Add(screening);
            _db.SaveChanges();
        }
    }
}
