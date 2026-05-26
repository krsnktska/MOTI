using System.ComponentModel.DataAnnotations.Schema;

namespace AirlineGame.Models;

public class GameConfig
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string Player1Name { get; set; } = string.Empty;
    public string Player2Name { get; set; } = string.Empty;

    public List<PlayerStrategy> Strategies { get; set; } = [];
    public List<MatrixCell> MatrixCells { get; set; } = [];
    public List<GameSession> Sessions { get; set; } = [];

    [NotMapped]
    public IEnumerable<PlayerStrategy> Player1Strategies => Strategies.Where(s => s.PlayerNumber == 1).OrderBy(s => s.Index);
    [NotMapped]
    public IEnumerable<PlayerStrategy> Player2Strategies => Strategies.Where(s => s.PlayerNumber == 2).OrderBy(s => s.Index);

    public int GetPayoff(int row, int col) =>
        MatrixCells.FirstOrDefault(c => c.Row == row && c.Col == col)?.Value ?? 0;
}
