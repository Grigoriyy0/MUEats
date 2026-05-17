using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using MUEats.Restaurants.Application.Handlers.Interfaces;
using MUEats.Restaurants.Core.Projections.Order;
using MUEats.Restaurants.Infrastructure.Persistence.Contexts;

namespace MUEats.Restaurants.Infrastructure.Workers;

internal sealed class OrderSnapshotCreatedWorker : BackgroundService
{
    private const int BatchSize = 50;
    private readonly TimeSpan _delay = TimeSpan.FromSeconds(1);
    private readonly IServiceScopeFactory _serviceScopeFactory;
    private const int MaxRetryCount = 3;

    public OrderSnapshotCreatedWorker(IServiceScopeFactory serviceScopeFactory)
    {
        _serviceScopeFactory = serviceScopeFactory;
    }

    protected override Task ExecuteAsync(CancellationToken ct)
    {
        return WorkingLoop(ct);
    }

    private async Task WorkingLoop(CancellationToken ct)
    {
        while (!ct.IsCancellationRequested)
        {
            ct.ThrowIfCancellationRequested();

            await using var scope = _serviceScopeFactory.CreateAsyncScope();

            var dbCtx = scope.ServiceProvider.GetRequiredService<RestaurantsDbContext>();

            await dbCtx.Database.BeginTransactionAsync(ct);

            var lockId = Guid.NewGuid();

            var idList = await dbCtx.Database.SqlQueryRaw<Guid>("""
                                                                      UPDATE "order_snapshots" SET "lock_id" = {0}
                                                                      WHERE "id" IN (
                                                                          SELECT "id" FROM "order_snapshots" WHERE 
                                                                          ("lock_id" is null or "lock_id" = {0}) and
                                                                          ("next_attempt_at" is null or "next_attempt_at" < {1}) and
                                                                          ("retry_count" < {2}) and
                                                                          ("status" = {3})
                                                                          ORDER BY "created_at"
                                                                          LIMIT {4}
                                                                          FOR UPDATE SKIP LOCKED
                                                                      ) 
                                                                      RETURNING "id"
                                                                      """, lockId, DateTime.UtcNow, MaxRetryCount, "Created", BatchSize)
                .ToListAsync(ct);

            await dbCtx.Database.CommitTransactionAsync(ct);

            if (idList.Count == 0)
            {
                await Task.Delay(_delay, ct);
                continue;
            }
            
            foreach (var snapshotId in idList)
            {
                await ProcessAsync(snapshotId, ct);
            }
        }
    }

    private async Task ProcessAsync(Guid snapshotId, CancellationToken ct)
    {
        await using var scope = _serviceScopeFactory.CreateAsyncScope();

        var handler = scope.ServiceProvider.GetRequiredService<IOrderSnapshotCreatedHandler>();

        await handler.HandleAsync(snapshotId, ct);
    }
}