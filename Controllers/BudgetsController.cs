using Byudzhetogramma.Data;
using Byudzhetogramma.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Byudzhetogramma.Controllers;

[Authorize]
public class BudgetsController : Controller
{
    private readonly ApplicationDbContext _db;
    private readonly UserManager<IdentityUser> _users;

    public BudgetsController(ApplicationDbContext db, UserManager<IdentityUser> users)
    {
        _db = db;
        _users = users;
    }

    public async Task<IActionResult> Index()
    {
        var userId = _users.GetUserId(User)!;
        var start = new DateTime(DateTime.Today.Year, DateTime.Today.Month, 1);
        var items = await _db.BudgetLimits
            .Where(x => x.UserId == userId && x.Month == start)
            .OrderBy(x => x.Category)
            .ToListAsync();
        return View(items);
    }

    [HttpGet]
    public IActionResult Create() => View(new BudgetLimit { Month = new DateTime(DateTime.Today.Year, DateTime.Today.Month, 1) });

    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(BudgetLimit model)
    {
        ModelState.Remove(nameof(BudgetLimit.UserId));
        if (!ModelState.IsValid) return View(model);
        var userId = _users.GetUserId(User)!;
        model.UserId = userId;
        model.Month = new DateTime(model.Month.Year, model.Month.Month, 1);

        var existing = await _db.BudgetLimits.FirstOrDefaultAsync(x =>
            x.UserId == userId && x.Month == model.Month && x.Category.ToLower() == model.Category.ToLower());

        if (existing is null) _db.BudgetLimits.Add(model);
        else existing.LimitAmount = model.LimitAmount;

        await _db.SaveChangesAsync();
        return RedirectToAction(nameof(Index));
    }

    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> Delete(int id)
    {
        var userId = _users.GetUserId(User)!;
        var item = await _db.BudgetLimits.FirstOrDefaultAsync(x => x.Id == id && x.UserId == userId);
        if (item is null) return NotFound();
        _db.BudgetLimits.Remove(item);
        await _db.SaveChangesAsync();
        return RedirectToAction(nameof(Index));
    }
}
