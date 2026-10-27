using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BudgetWise.Models;

public class MonthlyBudget
{
    [Key]
      public int Id { get; set; }
    public int BudgetId { get; set; }

    [Range(1, 12)]
    public int Month { get; set; }

    [Range(2000, 2100)]
    public int Year { get; set; }

    [Required]
    [Range(typeof(decimal), "0", "999999999", ErrorMessage = "Income cannot be negative.")]
    [Column(TypeName = "decimal(10,2)")]
    public decimal Income { get; set; }
}
