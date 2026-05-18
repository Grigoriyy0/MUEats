using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using MUEats.Restaurants.Application.Handlers.Interfaces;
using MUEats.Restaurants.Core.Projections.Order;
using MUEats.Restaurants.Infrastructure.Persistence.Contexts;

namespace MUEats.Restaurants.Infrastructure.Workers;

internal sealed class OrderCancellationJob : BackgroundService
{
    private readonly IServiceScopeFactory _scopeFactory;
    private const int BatchSize = 50;
    private readonly TimeSpan _delay = TimeSpan.FromSeconds(1);
    private readonly ILogger<OrderCancellationJob> _logger;
    
    public OrderCancellationJob(IServiceScopeFactory scopeFactory, ILogger<OrderCancellationJob> logger)
    {
        _scopeFactory = scopeFactory;
        _logger = logger;
    }

    protected override Task ExecuteAsync(CancellationToken ct)
    {
        return WorkingLoop(ct);
    }

    private async Task WorkingLoop(CancellationToken ct)
    {
        while (!ct.IsCancellationRequested)
        {
            try
            {
                await using var scope = _scopeFactory.CreateAsyncScope();

                var ctx = scope.ServiceProvider.GetRequiredService<RestaurantsDbContext>();

                await ctx.Database.BeginTransactionAsync(ct);

                var lockId = Guid.NewGuid();
            
                var ids = await ctx.Database.SqlQueryRaw<Guid>("""
                                                               UPDATE "order_snapshots" SET "lock_id" = {0}
                                                               WHERE "id" IN (
                                                                   SELECT "id" FROM "order_snapshots" WHERE
                                                                   ("created_at" <= {1}) AND
                                                                   ("lock_id" is null or "lock_id" = {0}) AND
                                                                   ("status" = {2}) AND
                                                                   ("next_attempt_at" < {3} or "next_attempt_at" is null)
                                                                   ORDER BY "created_at"
                                                                   LIMIT {4}
                                                                   FOR UPDATE SKIP LOCKED
                                                               ) RETURNING "id"
                                                               """, lockId, DateTime.UtcNow.AddMinutes(-10), "Pending", DateTime.UtcNow, BatchSize)
                    .ToListAsync(ct);

                await ctx.Database.CommitTransactionAsync(ct);

                if (ids.Count == 0)
                {
                    await Task.Delay(_delay, ct);
                    continue;
                }

                var snapshots = await ctx.OrderSnapshots.Where(x => ids.Contains(x.Id))
                    .ToListAsync(ct);
            
                foreach (var snapshot in snapshots)
                {
                    await ProcessAsync(snapshot, ct);
                }
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Error occured");
                await Task.Delay(_delay, ct);
            }
        }
    }

    private async Task ProcessAsync(OrderSnapshot snapshot, CancellationToken ct)
    {
        await using var scope = _scopeFactory.CreateAsyncScope();
        
        var handler = scope.ServiceProvider.GetRequiredService<IOrderSnapshotCancelHandler>();

        await handler.HandleAsync(snapshot, ct);
    }
}