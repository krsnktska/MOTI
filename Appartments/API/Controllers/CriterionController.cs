using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using Appartments.API.Models;
using Appartments.API.Data;

namespace Appartments.API.Controllers;

[Route("api/[controller]")]
[ApiController]
public class CriterionController : Controller
{
    private readonly AppDbContext _context;
    public CriterionController(AppDbContext context) => _context = context;

    [HttpGet("/Criteria/Index")] 
    public IActionResult Index() => View(); 

    [HttpGet]
    public async Task<ActionResult<IEnumerable<Criterion>>> GetCriteria() => await _context.Criteria.ToListAsync();

    [Authorize]
    [HttpPost]
    public async Task<ActionResult<Criterion>> PostCriterion(Criterion criterion)
    {
        _context.Criteria.Add(criterion);
        await _context.SaveChangesAsync();
        return Ok(criterion);
    }
    
    [Authorize]
    [HttpPut("{id}")]
    public async Task<IActionResult> PutCriterion(int id, Criterion criterion)
    {
        if (id != criterion.Criterion_id) return BadRequest();
        _context.Entry(criterion).State = EntityState.Modified;

        try
        {
            await _context.SaveChangesAsync();
        }
        catch (DbUpdateConcurrencyException)
        {
            if (!_context.Criteria.Any(e => e.Criterion_id == id)) return NotFound();
            else throw;
        }
        
        return NoContent();
    }

    [Authorize]
    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteCriterion(int id)
    {
        var crit = await _context.Criteria.FindAsync(id);
        if (crit == null) return NotFound();
        _context.Criteria.Remove(crit);
        await _context.SaveChangesAsync();
        return NoContent();
    }
}