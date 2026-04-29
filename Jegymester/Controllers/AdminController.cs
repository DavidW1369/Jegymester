using Jegymester.Context;
using Jegymester.Entites;
using Jegymester.Services;
using Microsoft.AspNetCore.Mvc;

namespace Jegymester.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AdminController : ControllerBase
    {
        private readonly AdminService _adminService;
        private readonly AppDbContext _context; // Szükséges a listák lekéréséhez

        public AdminController(AdminService adminService, AppDbContext context)
        {
            _adminService = adminService;
            _context = context;
        }

        [HttpPost("add-movie")]
        public IActionResult AddMovie([FromBody] Movie movie)
        {
            _adminService.AddMovie(movie);
            return Ok(new { message = "Film sikeresen hozzáadva!" });
        }

        [HttpDelete("delete-movie/{id}")]
        public IActionResult DeleteMovie(int id)
        {
            try
            {
                _adminService.DeleteMovie(id);
                return Ok(new { message = "Film törölve!" });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpPost("create-screening")]
        public IActionResult CreateScreening([FromBody] Screening screening)
        {
            try
            {
                _adminService.CreateScreening(screening);
                return Ok(new { message = "Vetítés létrehozva!" });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        // Segéd-végpontok a listákhoz
        [HttpGet("movies")]
        public IActionResult GetMovies() => Ok(_context.Movies.ToList());

        [HttpGet("rooms")]
        public IActionResult GetRooms() => Ok(_context.Rooms.ToList());
    }
}
