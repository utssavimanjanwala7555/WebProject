using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BudgetWise.Models;

public class Expense
{
    public int ExpenseId { get; set; }

    [Required(ErrorMessage = "Enter an expense title.")]
    [StringLength(100)]
    public string Title { get; set; } = string.Empty;

    [Required]
    [Range(typeof(decimal), "0.01", "999999999", ErrorMessage = "Amount must be greater than zero.")]
    [Column(TypeName = "decimal(10,2)")]
    public decimal Amount { get; set; }

    [Required]
    [DataType(DataType.Date)]
    [Display(Name = "Expense date")]
    public DateTime Date { get; set; } = DateTime.Today;

    [Required(ErrorMessage = "Choose a category.")]
    [Display(Name = "Category")]
    public int CategoryId { get; set; }

    [StringLength(500)]
    public string? Description { get; set; }

    public Category? Category { get; set; }
}
