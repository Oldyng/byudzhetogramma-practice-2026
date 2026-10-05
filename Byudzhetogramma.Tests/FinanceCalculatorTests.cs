using Xunit;
using Byudzhetogramma.Models;
using Byudzhetogramma.Services;

namespace Byudzhetogramma.Tests;

public class FinanceCalculatorTests
{
    private readonly FinanceCalculator _calculator = new();

    [Fact]
    public void CalculateBalance_SubtractsExpensesFromIncome()
    {
        var data = new[]
        {
            new FinanceTransaction { Type = TransactionType.Income, Amount = 1000m },
            new FinanceTransaction { Type = TransactionType.Expense, Amount = 250m },
            new FinanceTransaction { Type = TransactionType.Expense, Amount = 100m }
        };

        Assert.Equal(650m, _calculator.CalculateBalance(data));
    }

    [Fact]
    public void IsBudgetExceeded_ReturnsTrueWhenCategorySpendingIsAboveLimit()
    {
        var data = new[]
        {
            new FinanceTransaction { Type = TransactionType.Expense, Category = "Продукты", Amount = 700m },
            new FinanceTransaction { Type = TransactionType.Expense, Category = "Продукты", Amount = 500m }
        };
        var limit = new BudgetLimit { Category = "Продукты", LimitAmount = 1000m };

        Assert.True(_calculator.IsBudgetExceeded(data, limit));
    }
}
