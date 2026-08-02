using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using MUEats.Notifications.Core.Domain;
using MUEats.Notifications.Infrastructure.Persistence;

namespace MUEats.Notifications.Infrastructure.Workers;

public class NotificationProcessingWorker : BackgroundService
{
    private readonly IServiceScopeFactory _scopeFactory;
    private const int MaxRetryCount = 3;
    private readonly TimeSpan _defaultDelay = TimeSpan.FromMilliseconds(200);

    public NotificationProcessingWorker(IServiceScopeFactory scopeFactory)
    {
        _scopeFactory = scopeFactory;
    }

    protected override async Task ExecuteAsync(CancellationToken ct)
    {
        while (!ct.IsCancellationRequested)
        {
            await using var scope = _scopeFactory.CreateAsyncScope();

            var dbCtx = scope.ServiceProvider.GetRequiredService<NotificationsDbContext>();

            await dbCtx.Database.BeginTransactionAsync(ct);

            var lockId = Guid.NewGuid();
        
            var ids = await dbCtx.Database.SqlQueryRaw<Guid>("""
                                                             UPDATE "Notifications" SET "LockId" = {0} 
                                                             WHERE "Id" IN (
                                                                   SELECT "Id" FROM "Notifications" WHERE "Status" = {1} and
                                                                   ("NextAttemptAt" <= {2} or "NextAttemptAt" is null) and
                                                                   ("AttemptCount" < {3}) and
                                                                   ("LockId" is null or "LockId" = {0})
                                                                   ORDER BY "CreatedAt"
                                                                   LIMIT {4}
                                                             FOR UPDATE SKIP LOCKED) RETURNING "Id"
                                                             """, lockId, nameof(NotificationStatus.Created), DateTime.UtcNow, MaxRetryCount, 50)
                .ToListAsync(ct);

            await dbCtx.Database.CommitTransactionAsync(ct);

            if (ids.Count == 0)
            {
                await Task.Delay(_defaultDelay, ct);
                continue;
            }

            var notifications = await dbCtx.Notifications.Where(x => ids.Contains(x.Id))
                .ToListAsync(ct);

            foreach (var notification in notifications)
            {
                await ProcessNotificationAsync(notification, ct);
            }
        }
    }

    private async Task ProcessNotificationAsync(Notification notification, CancellationToken ct)
    {
        
    }
}