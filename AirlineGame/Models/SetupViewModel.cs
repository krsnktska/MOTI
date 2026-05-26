using AirlineGame.Models;
namespace AirlineGame.Models
{
    public class SetupViewModel
    {
        public GameConfig Game { get; set; } = null!;
        public int[][] Matrix { get; set; } = [];
    }
}
