using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using MUEats.Application.Helpers;
using MUEats.Application.Ports;
using MUEats.Core;
using MUEats.Core.Domain.Events;
using MUEats.Infrastructure.Persistence;
using Newtonsoft.Json;

namespace MUEats.Infrastructure.Workers;

internal sealed class OutboxProcessingWorker(IServiceScopeFactory serviceScopeFactory, IProducer producer) : BackgroundService
{
    private static readonly TimeSpan Delay = TimeSpan.FromMilliseconds(500);

    private const int BatchSize = 50;
    
    private const int RetryMaxCount = 3;
    
    protected override async Task ExecuteAsync(CancellationToken ct)
    {
        while (!ct.IsCancellationRequested)
        {
            await using var scope = serviceScopeFactory.CreateAsyncScope();

            var dbContext = scope.ServiceProvider.GetRequiredService<MueDbContext>();

            await dbContext.Database.BeginTransactionAsync(ct);
            
            var lockId = Guid.NewGuid();

            var idList = await dbContext.Database.SqlQueryRaw<Guid>("""
                                                                          UPDATE  "OutboxMessages" set "LockId" = {0}
                                                                          WHERE IN (
                                                                            SELECT "Id" FROM "OutboxMessages" WHERE "Status" == {1} and
                                                                              ("NextRetryAt" <= {2} or "NextRetryAt" is null) and
                                                                              ("RetryCount" < {3}) and
                                                                              ("LockId" is null or "LockId" = {0})
                                                                              ORDER BY "CreatedAt"
                                                                              LIMIT {4}
                                                                              FOR UPDATE SKIP LOCKED) RETURNING "Id" 
                                                                          """, lockId, nameof(OutboxStatus.Pending), DateTime.UtcNow, RetryMaxCount, BatchSize)
                .ToListAsync(ct);
            
            
            await dbContext.Database.CommitTransactionAsync(ct);
            
            if (idList.Count == 0)
            {
                await Task.Delay(Delay, ct);
                continue;
            }

            var messages = await dbContext.OutboxMessages
                .Where(x => idList.Contains(x.Id))
                .ToListAsync(ct);
            
            foreach (var message in messages)
            {
                await ProcessMessageAsync(message, ct);
            }
        }
    }

    private async Task ProcessMessageAsync(OutboxMessage message, CancellationToken ct)
    {
        await using var scope = serviceScopeFactory.CreateAsyncScope();

        var dbContext = scope.ServiceProvider.GetRequiredService<MueDbContext>();

        try
        {
            var @event = JsonConvert.DeserializeObject<DomainEvent>(message.JsonPayload, JsonSerializerHelper.Settings);

            if (@event is null)
            {
                return;
            }
        
            await producer.ProduceAsync(@event, ct);
            message.ProcessedAt = DateTime.UtcNow;
            message.Status = OutboxStatus.Processed;
        }
        catch (Exception e)
        {
            message.LastError = e.Message;
            message.RetryCount++;

            if (message.RetryCount >= RetryMaxCount)
            {
                message.Status = OutboxStatus.Failed;
            }
            else
            {
                message.NextRetryAt = DateTime.UtcNow.AddMinutes(1);
            }
        }
        
        await dbContext.SaveChangesAsync(ct);
    }
}