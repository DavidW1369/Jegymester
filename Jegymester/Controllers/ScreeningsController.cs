using Jegymester.Context;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Jegymester.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ScreeningsController : ControllerBase
    {
        private readonly AppDbContext _context;

        public ScreeningsController(AppDbContext context)
        {
            _context = context;
        }

        // ScreeningsController.cs
        [HttpGet]
        public IActionResult GetScreenings()
        {
            return Ok(_context.Screenings
                .Include(s => s.Movie)
                .Include(s => s.Room) // Beemeljük a Room tábla adatait is
                .ToList());
        }
    }
}