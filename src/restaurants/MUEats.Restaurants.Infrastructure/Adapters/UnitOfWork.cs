using MUEats.Restaurants.Application.Ports;
using MUEats.Restaurants.Infrastructure.Persistence.Contexts;

namespace MUEats.Restaurants.Infrastructure.Adapters;

public class UnitOfWork : IUnitOfWork
{
    private readonly RestaurantsDbContext _context;

    public UnitOfWork(RestaurantsDbContext context)
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

    public Task SaveChangesAsync(CancellationToken ct)
    {
        return _context.SaveChangesAsync(ct);
    }
}