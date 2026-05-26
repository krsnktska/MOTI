using AirlineGame.Models;
using System.Collections.Generic;
namespace AirlineGame.Models
{
    public class PlayViewModel
    {
        public GameConfig Game { get; set; } = null!;
        public GameSession? Session { get; set; }
        public GameRound? LastRound { get; set; }
        public List<GameRound> History { get; set; } = new();
        public int ChosenStrategy { get; set; }
        public int SessionId { get; set; }
        public (int StrategyIndex, int MinPayoff) MaximinResult { get; set; }
        public bool ShowMaximin { get; set; }
    }
}
