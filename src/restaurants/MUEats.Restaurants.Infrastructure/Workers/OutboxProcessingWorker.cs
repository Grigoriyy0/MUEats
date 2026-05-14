using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using MUEats.Restaurants.Application.Helpers;
using MUEats.Restaurants.Application.Ports;
using MUEats.Restaurants.Infrastructure.ExternalServices.IntegrationEvents;
using MUEats.Restaurants.Infrastructure.Persistence.Contexts;
using MUEats.Restaurants.Infrastructure.Persistence.Outbox;
using Newtonsoft.Json;

namespace MUEats.Restaurants.Infrastructure.Workers;

public class OutboxProcessingWorker : BackgroundService
{
    private static readonly TimeSpan Delay = TimeSpan.FromMilliseconds(500);
    private const int BatchSize = 50;
    private const int RetryMaxCount = 3;
    private readonly IServiceScopeFactory _serviceScopeFactory;
    private readonly IMessageBus _bus;

    public OutboxProcessingWorker(IServiceScopeFactory serviceScopeFactory, IMessageBus bus)
    {
        _serviceScopeFactory = serviceScopeFactory;
        _bus = bus;
    }

    protected override async Task ExecuteAsync(CancellationToken ct)
    {
        while (!ct.IsCancellationRequested)
        {
            await using var scope = _serviceScopeFactory.CreateAsyncScope();

            var dbContext = scope.ServiceProvider.GetRequiredService<RestaurantsDbContext>();

            await dbContext.Database.BeginTransactionAsync(ct);
            
            var lockId = Guid.NewGuid();

            var idList = await dbContext.Database.SqlQueryRaw<Guid>("""
                                                                          UPDATE  "OutboxMessages" set "LockId" = {0}
                                                                          WHERE "Id" IN (
                                                                            SELECT "Id" FROM "OutboxMessages" WHERE "Status" = {1} and
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
                message.LockId = null;
            }

            await dbContext.SaveChangesAsync(ct);
        }
    }

    private async Task ProcessMessageAsync(OutboxMessage message, CancellationToken ct)
    {
        try
        {
            var @event = JsonConvert.DeserializeObject<IntegrationEvent>(message.JsonPayload, JsonSerializerHelper.Settings);

            if (@event is null)
            {
                return;
            }

            await _bus.ProduceAsync(@event, ct);
            message.ProcessedAt = DateTime.UtcNow;
            message.Status = OutboxStatus.Processed;
        }
        catch (Exception e)
        {
            message.LastError = e.Message;
            message.AttemptsCount++;

            if (message.AttemptsCount >= RetryMaxCount)
            {
                message.Status = OutboxStatus.Failed;
            }
            else
            {
                var delay = Math.Pow(2, message.AttemptsCount - 1);
                message.NextAttemptAt = DateTime.UtcNow.AddMinutes(delay);
            }
        }
    }
}