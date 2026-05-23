using System.ComponentModel.DataAnnotations;

namespace Appartments.API.Models;

public class Result
{
    [Key]
    public int Result_id { get; set; }
    public int LPR_id { get; set; }
    public int Alternative_id { get; set; }
    public double? Score { get; set; }
}