namespace AirlineGame.Models;

public class MatrixCell
{
    public int Id { get; set; }
    public int GameConfigId { get; set; }
    public int Row { get; set; }
    public int Col { get; set; }
    public int Value { get; set; }

    public GameConfig GameConfig { get; set; } = null!;
}
