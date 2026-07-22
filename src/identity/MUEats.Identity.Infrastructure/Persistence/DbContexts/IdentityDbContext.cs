using Microsoft.EntityFrameworkCore;
using MUEats.Identity.Core.Domain.Role;
using MUEats.Identity.Core.Domain.User;

namespace MUEats.Identity.Infrastructure.Persistence.DbContexts;

public class IdentityDbContext : DbContext
{
    public IdentityDbContext(DbContextOptions<IdentityDbContext> options) : base(options)
    {
        
    } 
    
    public DbSet<User> Users { get; set; }
    
    public DbSet<Role> Roles { get; set; }
    
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(IdentityDbContext).Assembly);
    }
}