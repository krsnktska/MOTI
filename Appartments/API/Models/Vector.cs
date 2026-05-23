using System.ComponentModel.DataAnnotations;

namespace Appartments.API.Models;

public class Vector 
{
    [Key]
    public int Vector_id { get; set; }
    public int Alternative_id { get; set; }
    public int Criterion_id { get; set; }
    public string? Value { get; set; }
}