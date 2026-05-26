using AirlineGame.Data;
using AirlineGame.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace AirlineGame.Controllers
{
    public class HomeController : Controller
    {
        private readonly AppDbContext _db;

        public HomeController(AppDbContext db)
        {
            _db = db;
        }

        public IActionResult Index()
        {
            return View();
        }

        [HttpGet]
        public async Task<IActionResult> Setup()
        {
            var game = await LoadGame();
            var matrix = BuildMatrix(game);
            return View(new SetupViewModel { Game = game, Matrix = matrix });
        }

        [HttpPost]
        public async Task<IActionResult> Setup(SetupViewModel model)
        {
            var game = await LoadGame();
            
            for (int r = 0; r < game.MatrixCells.Count; r++)
            {
                var cell = game.MatrixCells.ElementAt(r);
                if (model.Matrix != null && cell.Row < model.Matrix.Length && cell.Col < model.Matrix[cell.Row].Length)
                {
                    cell.Value = model.Matrix[cell.Row][cell.Col];
                }
            }

            game.Player1Name = model.Game.Player1Name;
            game.Player2Name = model.Game.Player2Name;

            await _db.SaveChangesAsync();
            TempData["Success"] = "Матрицю збережено успішно.";
            return RedirectToAction("Setup");
        }

        [HttpGet]
        public async Task<IActionResult> Play(int? sessionId, bool showMaximin = false)
        {
            var game = await LoadGame();
            var model = new PlayViewModel
            {
                Game = game,
                ShowMaximin = showMaximin,
                MaximinResult = ComputeMaximin(game)
            };

            if (sessionId.HasValue)
            {
                model.Session = await _db.GameSessions
                    .Include(s => s.Rounds)
                    .FirstOrDefaultAsync(s => s.Id == sessionId);
                if (model.Session != null)
                {
                    model.History = model.Session.Rounds.OrderBy(r => r.RoundNumber).ToList();
                    model.LastRound = model.History.LastOrDefault();
                    model.SessionId = model.Session.Id;
                }
            }
            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> PlayMove(int ChosenStrategy, int SessionId)
        {
            var game = await LoadGame();
            GameSession session;
            if (SessionId > 0)
            {
                session = await _db.GameSessions
                    .Include(s => s.Rounds)
                    .FirstAsync(s => s.Id == SessionId);
            }
            else
            {
                session = new GameSession { GameConfigId = game.Id };
                _db.GameSessions.Add(session);
                await _db.SaveChangesAsync();
            }

            var rng = new Random();
            var p2strategies = game.Player2Strategies.ToList();
            int p2choice = rng.Next(p2strategies.Count);
            int payoff = game.GetPayoff(ChosenStrategy, p2choice);

            var round = new GameRound
            {
                GameSessionId = session.Id,
                RoundNumber = session.Rounds.Count + 1,
                Player1StrategyIndex = ChosenStrategy,
                Player2StrategyIndex = p2choice,
                Player1Payoff = payoff,
                Player2Payoff = -payoff
            };
            session.Rounds.Add(round);
            session.Player1TotalScore += payoff;
            session.Player2TotalScore += -payoff;
            await _db.SaveChangesAsync();

            return RedirectToAction("Play", new { sessionId = session.Id });
        }

        [HttpPost]
        public IActionResult NewSession()
        {
            return RedirectToAction("Play");
        }

        [HttpPost]
        public IActionResult ShowMaximin(int SessionId)
        {
            return RedirectToAction("Play", new { sessionId = SessionId > 0 ? SessionId : (int?)null, showMaximin = true });
        }

        public static int[][] BuildMatrix(GameConfig game) { var rows = game.Player1Strategies.Count(); var cols = game.Player2Strategies.Count(); var m = new int[rows][]; for (int r = 0; r < rows; r++) { m[r] = new int[cols]; for (int c = 0; c < cols; c++) m[r][c] = game.GetPayoff(r, c); } return m; } 
        private async Task<GameConfig> LoadGame() =>
            await _db.GameConfigs
                .Include(g => g.Strategies)
                .Include(g => g.MatrixCells)
                .FirstAsync();

        public static (int StrategyIndex, int MinPayoff) ComputeMaximin(GameConfig game)
        {
            var p1s = game.Player1Strategies.ToList();
            var p2s = game.Player2Strategies.ToList();
            int bestRow = 0;
            int bestMin = int.MinValue;
            for (int r = 0; r < p1s.Count; r++)
            {
                int rowMin = p2s.Select((_, c) => game.GetPayoff(r, c)).Min();
                if (rowMin > bestMin)
                {
                    bestMin = rowMin;
                    bestRow = r;
                }
            }
            return (bestRow, bestMin);
        }
    }
}
