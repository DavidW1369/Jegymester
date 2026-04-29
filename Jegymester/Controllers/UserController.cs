using Jegymester.DTOs;
using Jegymester.Entites;
using Jegymester.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Jegymester.Context;

[ApiController]
[Route("api/[controller]")]
public class UsersController : ControllerBase
{
    private readonly UserService _userService;
    public UsersController(UserService userService) => _userService = userService;

    [HttpPost("register")]
    public IActionResult Register([FromBody] RegisterDto dto)
    {
        try
        {
            _userService.RegisterUser(dto);
            return Ok(new { message = "Sikeres regisztráció!" });
        }
        catch (Exception ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    [HttpPost("login")]
    public IActionResult Login([FromBody] LoginDto dto)
    {
        var user = _userService.Login(dto.Email, dto.Password);

        if (user == null) return Unauthorized(new { message = "Hibás email vagy jelszó!" });

        // Itt kézzel összeállítjuk a választ, hogy a React értse
        return Ok(new
        {
            id = user.Id,
            name = user.Name,
            email = user.Email,
            phoneNumber = user.PhoneNumber,
            // Kényszerítsük a Role enum-ot számmá (int), és nevezzük roleId-nak
            roleId = (int)user.Role
        });
    }
}