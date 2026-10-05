using Byudzhetogramma.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace Byudzhetogramma.Data;

public class ApplicationDbContext : IdentityDbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options) { }

    public DbSet<FinanceTransaction> Transactions => Set<FinanceTransaction>();
    public DbSet<BudgetLimit> BudgetLimits => Set<BudgetLimit>();
}
