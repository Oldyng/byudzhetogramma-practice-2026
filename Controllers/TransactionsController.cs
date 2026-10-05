using System.Globalization;
using System.Text;
using Byudzhetogramma.Data;
using Byudzhetogramma.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Byudzhetogramma.Controllers;

[Authorize]
public class TransactionsController : Controller
{
    private readonly ApplicationDbContext _db;
    private readonly UserManager<IdentityUser> _users;

    public TransactionsController(ApplicationDbContext db, UserManager<IdentityUser> users)
    {
        _db = db;
        _users = users;
    }

    public async Task<IActionResult> Index()
    {
        var userId = _users.GetUserId(User)!;
        var items = await _db.Transactions
            .Where(x => x.UserId == userId)
            .OrderByDescending(x => x.Date)
            .ThenByDescending(x => x.Id)
            .ToListAsync();
        return View(items);
    }

    [HttpGet]
    public IActionResult Create() => View(new FinanceTransaction { Date = DateTime.Today });

    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(FinanceTransaction model)
    {
        ModelState.Remove(nameof(FinanceTransaction.UserId));
        if (!ModelState.IsValid) return View(model);
        model.UserId = _users.GetUserId(User)!;
        _db.Transactions.Add(model);
        await _db.SaveChangesAsync();
        return RedirectToAction(nameof(Index));
    }

    [HttpGet]
    public async Task<IActionResult> Edit(int id)
    {
        var item = await FindOwned(id);
        return item is null ? NotFound() : View(item);
    }

    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, FinanceTransaction model)
    {
        var item = await FindOwned(id);
        if (item is null) return NotFound();
        ModelState.Remove(nameof(FinanceTransaction.UserId));
        if (!ModelState.IsValid) return View(model);

        item.Date = model.Date;
        item.Description = model.Description;
        item.Category = model.Category;
        item.Amount = model.Amount;
        item.Type = model.Type;
        await _db.SaveChangesAsync();
        return RedirectToAction(nameof(Index));
    }

    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> Delete(int id)
    {
        var item = await FindOwned(id);
        if (item is null) return NotFound();
        _db.Transactions.Remove(item);
        await _db.SaveChangesAsync();
        return RedirectToAction(nameof(Index));
    }

    [HttpGet]
    public async Task<FileResult> ExportCsv()
    {
        var userId = _users.GetUserId(User)!;
        var items = await _db.Transactions.Where(x => x.UserId == userId).OrderBy(x => x.Date).ToListAsync();
        var sb = new StringBuilder();
        sb.AppendLine("Date;Type;Category;Description;Amount");
        foreach (var x in items)
        {
            var safeDescription = x.Description.Replace(";", ",");
            var safeCategory = x.Category.Replace(";", ",");
            sb.AppendLine($"{x.Date:yyyy-MM-dd};{x.Type};{safeCategory};{safeDescription};{x.Amount.ToString(CultureInfo.InvariantCulture)}");
        }
        return File(Encoding.UTF8.GetPreamble().Concat(Encoding.UTF8.GetBytes(sb.ToString())).ToArray(), "text/csv", "byudzhetogramma-operacii.csv");
    }

    private async Task<FinanceTransaction?> FindOwned(int id)
    {
        var userId = _users.GetUserId(User)!;
        return await _db.Transactions.FirstOrDefaultAsync(x => x.Id == id && x.UserId == userId);
    }
}
