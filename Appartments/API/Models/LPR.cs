using System.ComponentModel.DataAnnotations;

namespace Appartments.API.Models;

public class LPR
{
    [Key]
    public int LPR_id { get; set; }
    public required string Name { get; set; }
    public required string Password { get; set; }
    public double? Rank { get; set; }
}