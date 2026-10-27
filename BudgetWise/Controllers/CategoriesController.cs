using BudgetWise.Data;
using BudgetWise.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace BudgetWise.Controllers;

public class CategoriesController : Controller
{
    private readonly ApplicationDbContext _db;

    public CategoriesController(ApplicationDbContext db) => _db = db;

    public async Task<IActionResult> Index()
    {
        var categories = await _db.Categories
            .Include(c => c.Expenses)
            .OrderBy(c => c.Name)
            .ToListAsync();
        return View(categories);
    }

    [HttpGet]
    public IActionResult Create() => View(new Category());

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(Category category)
    {
        var name = category.Name.Trim();
        if (await _db.Categories.AnyAsync(c => c.Name.ToLower() == name.ToLower()))
            ModelState.AddModelError(nameof(category.Name), "A category with this name already exists.");

        if (!ModelState.IsValid) return View(category);

        category.Name = name;
        _db.Categories.Add(category);
        await _db.SaveChangesAsync();
        TempData["Success"] = "Category created successfully.";
        return RedirectToAction(nameof(Index));
    }

    [HttpGet]
    public async Task<IActionResult> Edit(int id)
    {
        var category = await _db.Categories.FindAsync(id);
        return category is null ? NotFound() : View(category);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, Category category)
    {
        if (id != category.CategoryId) return BadRequest();

        var name = category.Name.Trim();
        if (await _db.Categories.AnyAsync(c => c.CategoryId != id && c.Name.ToLower() == name.ToLower()))
            ModelState.AddModelError(nameof(category.Name), "A category with this name already exists.");

        if (!ModelState.IsValid) return View(category);

        var existing = await _db.Categories.FindAsync(id);
        if (existing is null) return NotFound();

        existing.Name = name;
        await _db.SaveChangesAsync();
        TempData["Success"] = "Category updated successfully.";
        return RedirectToAction(nameof(Index));
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Delete(int id)
    {
        var category = await _db.Categories.Include(c => c.Expenses)
            .FirstOrDefaultAsync(c => c.CategoryId == id);

        if (category is null) return NotFound();

        if (category.Expenses.Any())
        {
            TempData["Error"] = "This category is used by expenses. Reassign or delete those expenses before deleting it.";
            return RedirectToAction(nameof(Index));
        }

        _db.Categories.Remove(category);
        await _db.SaveChangesAsync();
        TempData["Success"] = "Category deleted successfully.";
        return RedirectToAction(nameof(Index));
    }
}
