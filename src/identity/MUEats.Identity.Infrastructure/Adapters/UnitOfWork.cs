using MUEats.Identity.Application.Ports;
using MUEats.Identity.Infrastructure.Persistence.DbContexts;

namespace MUEats.Identity.Infrastructure.Adapters;

public class UnitOfWork : IUnitOfWork
{
    private readonly IdentityDbContext _context;

    public UnitOfWork(IdentityDbContext context)
    {
        _context = context;
    }
    
    public async Task BeginTransactionAsync(CancellationToken ct)
    {
        await _context.Database.BeginTransactionAsync(ct);
    }

    public Task CommitTransactionAsync(CancellationToken ct)
    {
        return _context.Database.CommitTransactionAsync(ct);
    }

    public Task RollbackTransactionAsync(CancellationToken ct)
    {
        return _context.Database.RollbackTransactionAsync(ct);
    }

    public Task<int> SaveChangesAsync(CancellationToken ct)
    {
        return _context.SaveChangesAsync(ct);
    }
}