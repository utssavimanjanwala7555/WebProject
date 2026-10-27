using System.ComponentModel.DataAnnotations;

namespace BudgetWise.Models;

public class Category
{
    public int CategoryId { get; set; }

    [Required(ErrorMessage = "Enter a category name.")]
    [StringLength(50, ErrorMessage = "Category names must be 50 characters or fewer.")]
    [Display(Name = "Category name")]
    public string Name { get; set; } = string.Empty;

    public ICollection<Expense> Expenses { get; set; } = new List<Expense>();
}
