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

internal sealed class OutboxProcessingWorker(
    IServiceScopeFactory serviceScopeFactory,
    IProducer producer
    ) : BackgroundService
{
    private static readonly TimeSpan Delay = TimeSpan.FromMilliseconds(500);

    private const int BatchSize = 50;
    
    protected override async Task ExecuteAsync(CancellationToken ct)
    {
        while (!ct.IsCancellationRequested)
        {
            await using var scope = serviceScopeFactory.CreateAsyncScope();

            var dbContext = scope.ServiceProvider.GetRequiredService<MueDbContext>();

            var eventList = await dbContext.OutboxMessages
                .Where(x => x.ProcessedAt == null)
                .OrderBy(x => x.CreatedAt)
                .Take(BatchSize)
                .ToListAsync(ct);

            if (eventList.Count == 0)
            {
                await Task.Delay(Delay, ct);
                continue;
            }

            foreach (var message in eventList)
            {
                await ProcessMessageAsync(message, ct);
            }
        }
    }

    private async Task ProcessMessageAsync(OutboxMessage message, CancellationToken ct)
    {
        await using var scope = serviceScopeFactory.CreateAsyncScope();

        var dbContext = scope.ServiceProvider.GetRequiredService<MueDbContext>();

        var @event = JsonConvert.DeserializeObject<DomainEvent>(message.JsonPayload, JsonSerializerHelper.Settings);

        if (@event is null)
        {
            return;
        }
        
        message.ProcessedAt = DateTime.UtcNow;

        dbContext.Update(message);

        await dbContext.SaveChangesAsync(ct);
        
        await producer.ProduceAsync(@event, ct);
    }
}