using System.ComponentModel.DataAnnotations;

namespace Byudzhetogramma.Models;

public class BudgetLimit
{
    public int Id { get; set; }

    [Required]
    public string UserId { get; set; } = string.Empty;

    [Required, StringLength(50)]
    public string Category { get; set; } = string.Empty;

    [Range(0.01, 1_000_000_000)]
    public decimal LimitAmount { get; set; }

    [Required]
    [DataType(DataType.Date)]
    public DateTime Month { get; set; } = new(DateTime.Today.Year, DateTime.Today.Month, 1);
}
