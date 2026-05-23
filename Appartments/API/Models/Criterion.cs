using System.ComponentModel.DataAnnotations;

namespace Appartments.API.Models;

public class Criterion 
{
    [Key]
    public int Criterion_id { get; set; }
    public required string Name { get; set; } 
}