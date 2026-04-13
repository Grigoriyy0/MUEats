using MUEats.Application.Ports;
using MUEats.Infrastructure.Persistence;

namespace MUEats.Infrastructure.Adapters.Services;

public class UnitOfWork(MueDbContext context) : IUnitOfWork
{
    public async Task BeginTransactionAsync(CancellationToken ct)
    {
        await context.Database.BeginTransactionAsync(ct);
    }

    public Task CommitTransactionAsync(CancellationToken ct)
    {
        return context.Database.CommitTransactionAsync(ct);
    }

    public Task RollbackTransactionAsync(CancellationToken ct)
    {
        return context.Database.RollbackTransactionAsync(ct);
    }

    public Task SaveChangesAsync(CancellationToken ct)
    {
        return context.SaveChangesAsync(ct);
    }
}