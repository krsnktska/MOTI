namespace AirlineGame.Models;

public class PlayerStrategy
{
    public int Id { get; set; }
    public int GameConfigId { get; set; }
    public int PlayerNumber { get; set; }
    public int Index { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;

    public GameConfig GameConfig { get; set; } = null!;
}
