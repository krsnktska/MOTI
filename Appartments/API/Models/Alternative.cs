using System.ComponentModel.DataAnnotations;

namespace Appartments.API.Models;

public class Alternative 
{
    [Key]
    public int Alternative_id { get; set; }
    public required string Name { get; set; } 
}