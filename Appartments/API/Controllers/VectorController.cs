using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using Appartments.API.Models;
using Appartments.API.Data;

namespace Appartments.API.Controllers;

[Route("api/[controller]")]
[ApiController]
public class VectorController : Controller
{
    private readonly AppDbContext _context;
    public VectorController(AppDbContext context) => _context = context;

    [HttpGet]
    public async Task<ActionResult<IEnumerable<Vector>>> GetVectors()
    {
        return await _context.Vectors.ToListAsync();
    }

    [Authorize]
    [HttpPost] 
    public async Task<IActionResult> UpdateMark(Vector vector)
    {
        try {
            var existing = await _context.Vectors
                .FirstOrDefaultAsync(v => v.Alternative_id == vector.Alternative_id 
                                       && v.Criterion_id == vector.Criterion_id);

            if (existing != null) {
                if (string.IsNullOrWhiteSpace(vector.Value)) {
                    _context.Vectors.Remove(existing);
                } else {
                    existing.Value = vector.Value;
                }
            } else {
                if (!string.IsNullOrWhiteSpace(vector.Value)) {
                    _context.Vectors.Add(vector);
                }
            }

            await _context.SaveChangesAsync();
            return Ok();
        } catch (Exception ex) {
            return StatusCode(500, new { error = ex.Message, inner = ex.InnerException?.Message, stack = ex.StackTrace });
        }
    }

    [HttpGet("/Vectors/Evaluate")]
    public IActionResult Evaluate()
    {
        return View();
    }
}