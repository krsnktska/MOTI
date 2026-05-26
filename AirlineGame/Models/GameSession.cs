namespace AirlineGame.Models;

public class GameSession
{
    public int Id { get; set; }
    public int GameConfigId { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public int Player1TotalScore { get; set; }
    public int Player2TotalScore { get; set; }

    public GameConfig GameConfig { get; set; } = null!;
    public List<GameRound> Rounds { get; set; } = [];
}
