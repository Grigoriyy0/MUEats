using Microsoft.EntityFrameworkCore;
using MUEats.Discounts.Core.Discount;

namespace MUEats.Discounts.Infrastructure.Persistence.Contexts;

public class DiscountsDbContext : DbContext
{
    public DiscountsDbContext(DbContextOptions<DiscountsDbContext> options) : base(options)
    {
        
    }

    public DbSet<Discount> Discounts { get; set; }
    
    public DbSet<IdentityRole> IdentityRoles { get; set; }
    
    public DbSet<UserUsageCount> UserUsages { get; set; }
    
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(DiscountsDbContext).Assembly);
    }
}