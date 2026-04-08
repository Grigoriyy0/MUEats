using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using MUEats.Application.Helpers;
using MUEats.Core;
using MUEats.Core.Domain.Events.Order;
using MUEats.Infrastructure.Persistence;
using Newtonsoft.Json;

namespace MUEats.Infrastructure.Workers;

public class FakeKitchenWorker : BackgroundService
{
    private readonly IServiceScopeFactory _serviceScopeFactory;
    
    private Dictionary<Guid, DateTime> _orders = new();

    public FakeKitchenWorker(IServiceScopeFactory serviceScopeFactory)
    {
        _serviceScopeFactory = serviceScopeFactory;
    }

    public void AddOrder(Guid id)
    {
        _orders.Add(id, DateTime.UtcNow);
    }

    protected override async Task ExecuteAsync(CancellationToken ct)
    {
        while (!ct.IsCancellationRequested)
        {
            await WorkingLoop(ct);
        }
    }

    private async Task WorkingLoop(CancellationToken ct)
    {
        foreach (var order in _orders)
        {
            if (order.Value < DateTime.UtcNow.AddMinutes(-10))
            {
                continue;
            }
            
            await using var scope = _serviceScopeFactory.CreateAsyncScope();

            var context = scope.ServiceProvider.GetRequiredService<MueDbContext>();
                    
            var @event = new OrderPreparedEvent
            {
                OrderId = order.Key,
            };
            
            _orders.Remove(order.Key);
            
            var json = JsonConvert.SerializeObject(@event, JsonSerializerHelper.Settings);

            var outboxMessage = new OutboxMessage
            {
                Id = Guid.NewGuid(),
                JsonPayload = json,
                Type = @event.GetType().Name,
                CreatedAt = DateTime.UtcNow,
            };
                    
            await context.OutboxMessages.AddAsync(outboxMessage, ct);
            await context.SaveChangesAsync(ct);
        }
    }
}