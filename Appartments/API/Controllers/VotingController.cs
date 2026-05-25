using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Appartments.API.Data;

namespace Appartments.API.Controllers;

[Route("api/[controller]")]
[ApiController]
public class VotingController : Controller
{
    private readonly AppDbContext _context;
    public VotingController(AppDbContext context) => _context = context;

    [HttpGet("/Voting/Index")]
    public IActionResult Index() => View();

    [HttpGet("profile")]
    public async Task<ActionResult> GetVotingProfile()
    {
        var alternatives = await _context.Alternatives.OrderBy(a => a.Alternative_id).ToListAsync();
        var lprs = await _context.LPRs.ToListAsync();
        var results = await _context.Results.ToListAsync();
        int n = alternatives.Count;

        var completeLprIds = lprs
            .Select(l => l.LPR_id)
            .Where(id => results.Count(r => r.LPR_id == id) == n && n > 0)
            .ToList();

        if (!completeLprIds.Any() || n == 0)
            return Ok(new { alternatives = Array.Empty<object>(), oprs = Array.Empty<object>(), totalVoters = 0 });

        var oprRankings = completeLprIds.Select(lprId =>
        {
            var lpr = lprs.First(l => l.LPR_id == lprId);
            var rankedNames = results
                .Where(r => r.LPR_id == lprId)
                .OrderBy(r => r.Score)
                .Select(r => alternatives.First(a => a.Alternative_id == r.Alternative_id).Name)
                .ToList();
            return new { OprId = lprId, OprName = lpr.Name, RankedNames = rankedNames };
        }).ToList();

        return Ok(new
        {
            alternatives = alternatives.Select(a => new { a.Alternative_id, a.Name }),
            oprs = oprRankings,
            totalVoters = completeLprIds.Count
        });
    }

    [HttpGet("collective-decision")]
    public async Task<ActionResult> GetCollectiveDecision()
    {
        var alternatives = await _context.Alternatives.OrderBy(a => a.Alternative_id).ToListAsync();
        var results = await _context.Results.ToListAsync();
        int n = alternatives.Count;

        if (n < 2)
            return BadRequest(new { error = "Потрібно хоча б 2 альтернативи" });

        var lprIds = results.Select(r => r.LPR_id).Distinct().ToList();
        var completeLprIds = lprIds
            .Where(id => results.Count(r => r.LPR_id == id) == n)
            .ToList();

        if (completeLprIds.Count < 2)
            return Ok(new { error = "Недостатньо даних: потрібно мінімум 2 ЛПР з повним ранжуванням" });

        int totalVoters = completeLprIds.Count;

        var rankings = completeLprIds.ToDictionary(
            lprId => lprId,
            lprId => results
                .Where(r => r.LPR_id == lprId)
                .ToDictionary(r => r.Alternative_id, r => r.Score ?? 999)
        );

        var activeAlternatives = alternatives.Select(a => a.Alternative_id).ToList();
        var rounds = new List<object>();

        int currentRoundNum = 1;
        string winnerName = null;

        while (activeAlternatives.Count > 0)
        {
            // Count first choices among active alternatives
            var roundVotes = activeAlternatives.ToDictionary(a => a, a => 0);
            
            foreach (var lprId in completeLprIds)
            {
                var rank = rankings[lprId];
                var topChoiceId = activeAlternatives
                    .OrderBy(a => rank.TryGetValue(a, out var s) ? s : 999)
                    .First();
                roundVotes[topChoiceId]++;
            }

            var roundResults = roundVotes
                .Select(kv => new {
                    alternativeId = kv.Key,
                    alternativeName = alternatives.First(a => a.Alternative_id == kv.Key).Name,
                    votes = kv.Value
                })
                .OrderByDescending(x => x.votes)
                .ToList();

            var leader = roundResults.First();
            bool hasMajority = leader.votes > totalVoters / 2.0;
            
            // Identify who is eliminated
            int minVotes = roundResults.Last().votes;
            var eliminatedId = roundResults.Last().alternativeId;
            var eliminatedName = roundResults.Last().alternativeName;

            rounds.Add(new { 
                roundNumber = currentRoundNum, 
                results = roundResults,
                eliminatedName = hasMajority || activeAlternatives.Count == 1 ? null : eliminatedName
            });

            if (hasMajority || activeAlternatives.Count == 1)
            {
                winnerName = leader.alternativeName;
                break;
            }

            if (roundResults.All(r => r.votes == minVotes))
            {
                winnerName = "Нічия (однакова кількість голосів у всіх кандидатів)";
                break;
            }

            activeAlternatives.Remove(eliminatedId);
            currentRoundNum++;
        }

        return Ok(new
        {
            totalVoters,
            rounds,
            winner = winnerName,
            winnerRound = currentRoundNum
        });
    }
}
