using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Appartments.API.Models;
using Appartments.API.Data;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using System.Security.Claims;

namespace Appartments.API.Controllers;

[Route("api/[controller]")]
[ApiController]
public class LprController : Controller
{
    private readonly AppDbContext _context;

    public LprController(AppDbContext context) => _context = context;

    [HttpGet("current")]
    public IActionResult GetCurrent()
    {
        if (User.Identity?.IsAuthenticated == true)
        {
            return Ok(new
            {
                LPR_id = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "0"),
                Name = User.Identity.Name
            });
        }
        return Unauthorized();
    }

    [HttpGet("/Lpr/Create")]
    public IActionResult Create()
    {
        return View();
    }

    [HttpGet("/Lpr/Profile")]
    public IActionResult Profile()
    {
        if (User.Identity?.IsAuthenticated != true)
            return Redirect("/Lpr/Create");
        return View();
    }

    [HttpPost]
    public async Task<ActionResult<LPR>> CreateLpr(LPR lpr)
    {
        _context.LPRs.Add(lpr);
        await _context.SaveChangesAsync();

        var claims = new List<Claim> {
            new Claim(ClaimTypes.NameIdentifier, lpr.LPR_id.ToString()),
            new Claim(ClaimTypes.Name, lpr.Name)
        };
        var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
        await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, new ClaimsPrincipal(identity));

        return Ok(lpr);
    }

    [HttpPost("login")]
    public async Task<ActionResult<LPR>> Login([FromBody] LoginRequest request)
    {
        var lpr = await _context.LPRs
            .FirstOrDefaultAsync(l => l.Name == request.Name && l.Password == request.Password);

        if (lpr == null) return Unauthorized("Неправильне ім'я або пароль");

        var claims = new List<Claim> {
            new Claim(ClaimTypes.NameIdentifier, lpr.LPR_id.ToString()),
            new Claim(ClaimTypes.Name, lpr.Name)
        };
        var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
        await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, new ClaimsPrincipal(identity));

        return Ok(lpr);
    }

    [HttpPost("logout")]
    public async Task<IActionResult> Logout()
    {
        await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
        return Ok();
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateLpr(int id, LPR lpr)
    {
        if (id != lpr.LPR_id)
        {
            return BadRequest("ID у запиті не збігається з ID об'єкта");
        }

        _context.Entry(lpr).State = EntityState.Modified;

        try
        {
            await _context.SaveChangesAsync();
        }
        catch (DbUpdateConcurrencyException)
        {
            if (!_context.LPRs.Any(e => e.LPR_id == id)) return NotFound();
            else throw;
        }

        return NoContent();
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteLpr(int id)
    {
        var lpr = await _context.LPRs.FindAsync(id);
        if (lpr == null) return NotFound();

        _context.LPRs.Remove(lpr);
        await _context.SaveChangesAsync();
        return NoContent();
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<LPR>>> GetLprs()
    {
        return await _context.LPRs.ToListAsync();
    }
}

public class LoginRequest
{
    public required string Name { get; set; }
    public required string Password { get; set; }
}