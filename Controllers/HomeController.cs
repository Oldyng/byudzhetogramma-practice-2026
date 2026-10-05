using Byudzhetogramma.Data;
using Byudzhetogramma.Models;
using Byudzhetogramma.Services;
using Byudzhetogramma.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Byudzhetogramma.Controllers;

[Authorize]
public class HomeController : Controller
{
    private readonly ApplicationDbContext _db;
    private readonly UserManager<IdentityUser> _users;
    private readonly FinanceCalculator _calculator;

    public HomeController(ApplicationDbContext db, UserManager<IdentityUser> users, FinanceCalculator calculator)
    {
        _db = db;
        _users = users;
        _calculator = calculator;
    }

    public async Task<IActionResult> Index()
    {
        var userId = _users.GetUserId(User)!;
        var start = new DateTime(DateTime.Today.Year, DateTime.Today.Month, 1);
        var end = start.AddMonths(1);

        var transactions = await _db.Transactions
            .Where(x => x.UserId == userId && x.Date >= start && x.Date < end)
            .OrderByDescending(x => x.Date)
            .ToListAsync();

        var limits = await _db.BudgetLimits
            .Where(x => x.UserId == userId && x.Month == start)
            .OrderBy(x => x.Category)
            .ToListAsync();

        var vm = new DashboardViewModel
        {
            Income = _calculator.CalculateIncome(transactions),
            Expenses = _calculator.CalculateExpenses(transactions),
            Balance = _calculator.CalculateBalance(transactions),
            RecentTransactions = transactions.Take(8).ToList(),
            Budgets = limits.Select(x => new BudgetStatus
            {
                Category = x.Category,
                Limit = x.LimitAmount,
                Spent = _calculator.CalculateCategoryExpenses(transactions, x.Category)
            }).ToList()
        };

        return View(vm);
    }
}
