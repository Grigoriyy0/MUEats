using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using MUEats.Application.Helpers;
using MUEats.Application.Ports;
using MUEats.Core;
using MUEats.Infrastructure.IntegrationEvents;
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
        this._scopeFactory = scopeFactory;
    }

    protected override async Task ExecuteAsync(CancellationToken ct)
    {
        while (!ct.IsCancellationRequested)
        {
            await using var scope = _scopeFactory.CreateAsyncScope();

            var dbContext = scope.ServiceProvider.GetRequiredService<MueDbContext>();

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
                await ProcessMessageAsync(message, ct);

                message.LockId = null;
            }

            await dbContext.SaveChangesAsync(ct);
        }
    }

    private async Task ProcessMessageAsync(InboxMessage message, CancellationToken ct)
    {
        await using var scope = _scopeFactory.CreateAsyncScope();
        
        try
        {
            var @event = JsonConvert.DeserializeObject<IntegrationEvent>(message.JsonPayload, JsonSerializerHelper.Settings);

            if (@event is null)
            {
                return;
            }

            var dispatcher = scope.ServiceProvider.GetRequiredService<IEventDispatcher>();
            
            // todo
            
            message.ProcessedAt = DateTime.UtcNow;
            message.Status = InboxStatus.Processed;
        }
        catch (Exception e)
        {
            message.LastError = e.Message;
            message.RetryCount++;

            if (message.RetryCount >= RetryMaxCount)
            {
                message.Status = InboxStatus.Failed;
            }
            else
            {
                var delay = Math.Pow(2, message.RetryCount - 1);
                message.NextRetryAt = DateTime.UtcNow.AddMinutes(delay);
            }
        }
    }
}