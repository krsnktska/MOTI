using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using Appartments.API.Models;
using Appartments.API.Data;

namespace Appartments.API.Controllers;
[Route("api/[controller]")]
[ApiController]
public class AlternativeController : Controller
{
    private readonly AppDbContext _context;
    public AlternativeController(AppDbContext context) => _context = context;

    [HttpGet("/Alternatives/Index")] 
    public IActionResult Index()
    {
        return View(); 
    }
    
    [Authorize]
    [HttpPost] 
    public async Task<ActionResult<Alternative>> PostAlternative([FromBody] Alternative alt)
    {
        if (alt == null) return BadRequest();
        
        _context.Alternatives.Add(alt);
        await _context.SaveChangesAsync();
        return Ok(alt);
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<Alternative>>> GetAlternatives()
    {
        return await _context.Alternatives.ToListAsync();
    } 

    [Authorize]
    [HttpPut("{id}")]  
    public async Task<IActionResult> PutAlternative(int id, Alternative alt)
    {
        if (id != alt.Alternative_id) return BadRequest();
        _context.Entry(alt).State = EntityState.Modified;
        await _context.SaveChangesAsync();
        return NoContent();
    }

    [Authorize]
    [HttpDelete("{id}")] 
    public async Task<IActionResult> DeleteAlternative(int id)
    {
        var alt = await _context.Alternatives.FindAsync(id);
        if (alt == null) return NotFound();
        _context.Alternatives.Remove(alt);
        await _context.SaveChangesAsync();
        return NoContent();
    }
}