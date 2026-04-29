using Jegymester.DTOs;
using Jegymester.Entites;
using Jegymester.Entites;
using Jegymester.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

[ApiController]
[Route("api/[controller]")]
public class TicketsController : ControllerBase
{
    private readonly UserService _userService;
    private readonly CashierService _cashierService;

    public TicketsController(UserService userService, CashierService cashierService)
    {
        _userService = userService;
        _cashierService = cashierService; // Itt rendeljük hozzá a változóhoz
    }

    [HttpGet("my-orders/{email}")]
    public IActionResult GetOrders(string email)
    {
        return Ok(_userService.GetOrdersByEmail(email));
    }

    [HttpPost("purchase")]
    public IActionResult Purchase([FromBody] PurchaseDto request)
    {
        try
        {
            var order = new TicketOrder
            {
                Email = request.Email,
                PhoneNumber = request.PhoneNumber,
                PurchaseTime = DateTime.Now,
                Tickets = request.Seats.Select(s => new Ticket
                {
                    ScreeningId = request.ScreeningId,
                    Seat = s
                }).ToList()
            };

            _userService.PurchaseTickets(order);
            return Ok(new { message = "Sikeres vásárlás!" });
        }
        catch (Exception ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    [HttpDelete("{id}")]
    public IActionResult Cancel(int id)
    {
        try
        {
            _userService.CancelTicket(id);
            return Ok(new { message = "Jegy törölve!" });
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }
    }
    // TicketsController.cs
    [HttpGet("booked/{screeningId}")]
    public IActionResult GetBookedSeats(int screeningId)
    {
        try
        {
            // A _context helyett a _userService-t hívjuk meg
            var bookedSeats = _userService.GetBookedSeats(screeningId);
            return Ok(bookedSeats);
        }
        catch (Exception ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }
    // TicketsController.cs

    [HttpGet("my-tickets/{email}")]
    public IActionResult GetMyTickets(string email)
    {
        var orders = _userService.GetOrdersByEmail(email);
        return Ok(orders);
    }

    [HttpDelete("cancel/{ticketId}")]
    public IActionResult CancelTicket(int ticketId)
    {
        try
        {
            _userService.CancelTicket(ticketId);
            return Ok(new { message = "Jegy sikeresen törölve!" });
        }
        catch (Exception ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }
    [HttpPost("cashier-purchase")]
    public IActionResult CashierPurchase([FromBody] PurchaseDto request)
    {
        // A logikád: ha nincs megadva vásárló email, akkor a pénztárosé lesz
        var order = new TicketOrder
        {
            Email = request.Email,
            PhoneNumber = request.PhoneNumber ?? "HELYSZÍNI VÁSÁRLÁS",
            PurchaseTime = DateTime.Now,
            Tickets = request.Seats.Select(s => new Ticket
            {
                ScreeningId = request.ScreeningId,
                Seat = s
            }).ToList()
        };

        _cashierService.ProcessCashierPurchase(order);
        return Ok(new { message = "Helyszíni eladás sikeres!" });
    }
    [HttpPost("validate/{id}")]
    public IActionResult Validate(int id)
    {
        var result = _cashierService.ValidateTicket(id);
        if (result.StartsWith("OK")) return Ok(new { message = result });
        return BadRequest(new { message = result });
    }
}