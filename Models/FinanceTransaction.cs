using System.ComponentModel.DataAnnotations;

namespace Byudzhetogramma.Models;

public enum TransactionType
{
    Income = 1,
    Expense = 2
}

public class FinanceTransaction
{
    public int Id { get; set; }

    [Required]
    public string UserId { get; set; } = string.Empty;

    [Required]
    [DataType(DataType.Date)]
    public DateTime Date { get; set; } = DateTime.Today;

    [Required, StringLength(100)]
    public string Description { get; set; } = string.Empty;

    [Required, StringLength(50)]
    public string Category { get; set; } = string.Empty;

    [Range(0.01, 1_000_000_000)]
    public decimal Amount { get; set; }

    [Required]
    public TransactionType Type { get; set; }
}
