using Byudzhetogramma.Models;

namespace Byudzhetogramma.Services;

public class FinanceCalculator
{
    public decimal CalculateIncome(IEnumerable<FinanceTransaction> transactions) =>
        transactions.Where(x => x.Type == TransactionType.Income).Sum(x => x.Amount);

    public decimal CalculateExpenses(IEnumerable<FinanceTransaction> transactions) =>
        transactions.Where(x => x.Type == TransactionType.Expense).Sum(x => x.Amount);

    public decimal CalculateBalance(IEnumerable<FinanceTransaction> transactions) =>
        CalculateIncome(transactions) - CalculateExpenses(transactions);

    public decimal CalculateCategoryExpenses(IEnumerable<FinanceTransaction> transactions, string category) =>
        transactions
            .Where(x => x.Type == TransactionType.Expense &&
                        string.Equals(x.Category, category, StringComparison.OrdinalIgnoreCase))
            .Sum(x => x.Amount);

    public bool IsBudgetExceeded(IEnumerable<FinanceTransaction> transactions, BudgetLimit limit) =>
        CalculateCategoryExpenses(transactions, limit.Category) > limit.LimitAmount;
}
