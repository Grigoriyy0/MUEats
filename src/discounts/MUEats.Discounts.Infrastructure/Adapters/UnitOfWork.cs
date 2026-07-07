using MUEats.Discounts.Application.Ports;
using MUEats.Discounts.Infrastructure.Persistence.Contexts;

namespace MUEats.Discounts.Infrastructure.Adapters;

public class UnitOfWork : IUnitOfWork
{
    private readonly DiscountsDbContext _context;

    public UnitOfWork(DiscountsDbContext context)
    {
        _context = context;
    }

    public Task BeginTransactionAsync(CancellationToken ct)
    {
        return _context.Database.BeginTransactionAsync(ct);
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