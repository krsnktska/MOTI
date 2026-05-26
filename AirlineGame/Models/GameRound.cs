namespace AirlineGame.Models;

public class GameRound
{
    public int Id { get; set; }
    public int GameSessionId { get; set; }
    public int RoundNumber { get; set; }
    public int Player1StrategyIndex { get; set; }
    public int Player2StrategyIndex { get; set; }
    public int Player1Payoff { get; set; }
    public int Player2Payoff { get; set; }

    public GameSession GameSession { get; set; } = null!;
}
