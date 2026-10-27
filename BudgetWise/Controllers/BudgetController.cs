using BudgetWise.Data;
using BudgetWise.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace BudgetWise.Controllers;

public class BudgetController : Controller
{
    private readonly ApplicationDbContext _db;

    public BudgetController(ApplicationDbContext db) => _db = db;

    public async Task<IActionResult> Index(int? month, int? year, int? categoryId)
    {
        var selectedMonth = month is >= 1 and <= 12 ? month.Value : DateTime.Today.Month;
        var selectedYear = year is >= 2000 and <= 2100 ? year.Value : DateTime.Today.Year;

        var expensesQuery = _db.Expenses
            .Include(e => e.Category)
            .Where(e => e.Date.Month == selectedMonth && e.Date.Year == selectedYear);

        if (categoryId.HasValue && categoryId.Value > 0)
            expensesQuery = expensesQuery.Where(e => e.CategoryId == categoryId.Value);

        var expenses = await expensesQuery.OrderByDescending(e => e.Date).ToListAsync();
        var budget = await _db.MonthlyBudgets
            .FirstOrDefaultAsync(b => b.Month == selectedMonth && b.Year == selectedYear);

        ViewBag.Month = selectedMonth;
        ViewBag.Year = selectedYear;
        ViewBag.CategoryId = categoryId;
        ViewBag.Categories = await _db.Categories.OrderBy(c => c.Name).ToListAsync();
        ViewBag.Income = budget?.Income ?? 0m;
        ViewBag.TotalExpenses = expenses.Sum(e => e.Amount);
        ViewBag.Balance = (budget?.Income ?? 0m) - expenses.Sum(e => e.Amount);

        return View(expenses);
    }

    [HttpGet]
    public async Task<IActionResult> SetIncome(int? month, int? year)
    {
        var m = month is >= 1 and <= 12 ? month.Value : DateTime.Today.Month;
        var y = year is >= 2000 and <= 2100 ? year.Value : DateTime.Today.Year;
        var budget = await _db.MonthlyBudgets.FirstOrDefaultAsync(b => b.Month == m && b.Year == y)
                     ?? new MonthlyBudget { Month = m, Year = y, Income = 0 };
        return View(budget);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> SetIncome(MonthlyBudget model)
    {
        if (!ModelState.IsValid) return View(model);

        var existing = await _db.MonthlyBudgets
            .FirstOrDefaultAsync(b => b.Month == model.Month && b.Year == model.Year);

        if (existing is null) _db.MonthlyBudgets.Add(model);
        else existing.Income = model.Income;

        await _db.SaveChangesAsync();
        TempData["Success"] = "Monthly income saved successfully.";
        return RedirectToAction(nameof(Index), new { month = model.Month, year = model.Year });
    }

    [HttpGet]
    public async Task<IActionResult> Create()
    {
        await LoadCategories();
        return View(new Expense { Date = DateTime.Today });
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(Expense expense)
    {
        if (!await _db.Categories.AnyAsync(c => c.CategoryId == expense.CategoryId))
            ModelState.AddModelError(nameof(expense.CategoryId), "Choose a valid category.");

        if (!ModelState.IsValid)
        {
            await LoadCategories();
            return View(expense);
        }

        _db.Expenses.Add(expense);
        await _db.SaveChangesAsync();
        TempData["Success"] = "Expense added successfully.";
        return RedirectToAction(nameof(Index), new { month = expense.Date.Month, year = expense.Date.Year });
    }

    [HttpGet]
    public async Task<IActionResult> Edit(int id)
    {
        var expense = await _db.Expenses.FindAsync(id);
        if (expense is null) return NotFound();
        await LoadCategories();
        return View(expense);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, Expense expense)
    {
        if (id != expense.ExpenseId) return BadRequest();

        if (!await _db.Categories.AnyAsync(c => c.CategoryId == expense.CategoryId))
            ModelState.AddModelError(nameof(expense.CategoryId), "Choose a valid category.");

        if (!ModelState.IsValid)
        {
            await LoadCategories();
            return View(expense);
        }

        var existing = await _db.Expenses.FindAsync(id);
        if (existing is null) return NotFound();

        existing.Title = expense.Title;
        existing.Amount = expense.Amount;
        existing.Date = expense.Date;
        existing.CategoryId = expense.CategoryId;
        existing.Description = expense.Description;

        await _db.SaveChangesAsync();
        TempData["Success"] = "Expense updated successfully.";
        return RedirectToAction(nameof(Index), new { month = existing.Date.Month, year = existing.Date.Year });
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Delete(int id)
    {
        var expense = await _db.Expenses.FindAsync(id);
        if (expense is null) return NotFound();

        var month = expense.Date.Month;
        var year = expense.Date.Year;
        _db.Expenses.Remove(expense);
        await _db.SaveChangesAsync();
        TempData["Success"] = "Expense deleted successfully.";
        return RedirectToAction(nameof(Index), new { month, year });
    }

    private async Task LoadCategories()
    {
        ViewBag.Categories = await _db.Categories.OrderBy(c => c.Name).ToListAsync();
    }
}
