using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using MUEats.Application.Helpers;
using MUEats.Application.IntegrationEvents;
using MUEats.Application.Ports;
using MUEats.Core;
using MUEats.Infrastructure.Adapters.Services;
using MUEats.Infrastructure.Metrics;
using MUEats.Infrastructure.Persistence;
using Newtonsoft.Json;

namespace MUEats.Infrastructure.Workers;

internal sealed class InboxProcessingWorker : BackgroundService
{
    private static readonly TimeSpan Delay = TimeSpan.FromMilliseconds(500);
    private const int BatchSize = 50;
    private const int RetryMaxCount = 3;
    private readonly IServiceScopeFactory _scopeFactory;

    public InboxProcessingWorker(IServiceScopeFactory scopeFactory)
    {
        _scopeFactory = scopeFactory;
    }

    protected override async Task ExecuteAsync(CancellationToken ct)
    {
        while (!ct.IsCancellationRequested)
        {
            await using var scope = _scopeFactory.CreateAsyncScope();

            var dbContext = scope.ServiceProvider.GetRequiredService<MueDbContext>();
            var inboxService = scope.ServiceProvider.GetRequiredService<IInboxService>();
            
            await dbContext.Database.BeginTransactionAsync(ct);
            
            var lockId = Guid.NewGuid();

            var idList = await dbContext.Database.SqlQueryRaw<Guid>("""
                                                                          UPDATE  "InboxMessages" set "LockId" = {0}
                                                                          WHERE "Id" IN (
                                                                            SELECT "Id" FROM "InboxMessages" WHERE "Status" = {1} and
                                                                              ("NextRetryAt" <= {2} or "NextRetryAt" is null) and
                                                                              ("RetryCount" < {3}) and
                                                                              ("LockId" is null or "LockId" = {0})
                                                                              ORDER BY "CreatedAt"
                                                                              LIMIT {4}
                                                                              FOR UPDATE SKIP LOCKED) RETURNING "Id" 
                                                                          """, lockId, nameof(InboxStatus.Pending), DateTime.UtcNow, RetryMaxCount, BatchSize)
                .ToListAsync(ct);
            
            
            await dbContext.Database.CommitTransactionAsync(ct);
            
            if (idList.Count == 0)
            {
                await Task.Delay(Delay, ct);
                continue;
            }

            var messages = await dbContext.InboxMessages
                .Where(x => idList.Contains(x.Id))
                .ToListAsync(ct);
            
            foreach (var message in messages)
            {
                await inboxService.ProcessMessageAsync(message, ct);
            }
        }
    }
}