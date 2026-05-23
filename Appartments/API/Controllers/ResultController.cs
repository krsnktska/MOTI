using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Appartments.API.Models;
using Appartments.API.Data;
using Microsoft.AspNetCore.Authorization;

namespace Appartments.API.Controllers;

[Route("api/[controller]")]
[ApiController]
public class ResultController : Controller
{
    private readonly AppDbContext _context;
    public ResultController(AppDbContext context) => _context = context;

    [HttpGet]
    public async Task<ActionResult> GetResults()
    {
        var results = await (from r in _context.Results
                            join l in _context.LPRs on r.LPR_id equals l.LPR_id
                            join a in _context.Alternatives on r.Alternative_id equals a.Alternative_id
                            select new
                            {
                                r.Result_id,
                                r.LPR_id,
                                r.Alternative_id,
                                LprName = l.Name,
                                AlternativeName = a.Name,
                                Score = r.Score
                            }).ToListAsync();

        var grouped = results.GroupBy(r => new { r.Alternative_id, r.AlternativeName })
                            .Select(g => new
                            {
                                AlternativeId = g.Key.Alternative_id,
                                AlternativeName = g.Key.AlternativeName,
                                Ratings = g.Select(x => new {
                                    x.Result_id,
                                    x.LPR_id,
                                    LprName = x.LprName,
                                    Score = x.Score
                                })
                                .OrderBy(x => x.Score)
                                .ToList()
                            })
                            .OrderBy(x => x.AlternativeId)
                            .ToList();

        return Ok(grouped);
    }

    [Authorize]
    [HttpDelete("my/{alternativeId}")]
    public async Task<IActionResult> DeleteMyResult(int alternativeId)
    {
        var lprIdClaim = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier);
        if (lprIdClaim == null) return Unauthorized();
        int lprId = int.Parse(lprIdClaim.Value);

        var result = await _context.Results
            .FirstOrDefaultAsync(r => r.LPR_id == lprId && r.Alternative_id == alternativeId);

        if (result == null) return NotFound();

        _context.Results.Remove(result);
        await _context.SaveChangesAsync();

        return NoContent();
    }

    [Authorize]
    [HttpPost]
    public async Task<ActionResult<Result>> PostResult(Result result)
    {
        var lprIdClaim = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier);
        if (lprIdClaim == null) return Unauthorized();
        
        int lprId = int.Parse(lprIdClaim.Value);
        result.LPR_id = lprId;

        var existing = await _context.Results
            .FirstOrDefaultAsync(r => r.LPR_id == lprId && r.Alternative_id == result.Alternative_id);

        if (existing != null)
        {
            existing.Score = result.Score;
            _context.Entry(existing).State = EntityState.Modified;
        }
        else
        {
            _context.Results.Add(result);
        }

        await _context.SaveChangesAsync();
        return Ok(result);
    }

    [Authorize]
    [HttpGet("my")]
    public async Task<ActionResult<IEnumerable<Result>>> GetMyResults()
    {
        var lprIdClaim = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier);
        if (lprIdClaim == null) return Unauthorized();
        int lprId = int.Parse(lprIdClaim.Value);

        return await _context.Results
            .Where(r => r.LPR_id == lprId)
            .ToListAsync();
    }

    [HttpGet("/Result/Index")]
    public IActionResult Index()
    {
        return View();
    }

    [Authorize]
    [HttpGet("/Result/Compare")]
    public IActionResult Compare()
    {
        return View();
    }
}