using Byudzhetogramma.Models;

namespace Byudzhetogramma.ViewModels;

public class BudgetStatus
{
    public string Category { get; set; } = string.Empty;
    public decimal Spent { get; set; }
    public decimal Limit { get; set; }
    public bool IsExceeded => Spent > Limit;
}

public class DashboardViewModel
{
    public decimal Income { get; set; }
    public decimal Expenses { get; set; }
    public decimal Balance { get; set; }
    public List<FinanceTransaction> RecentTransactions { get; set; } = [];
    public List<BudgetStatus> Budgets { get; set; } = [];
}
