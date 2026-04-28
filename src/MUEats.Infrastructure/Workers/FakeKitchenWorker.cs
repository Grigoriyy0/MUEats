using System.Collections.Concurrent;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using MUEats.Application.Helpers;
using MUEats.Core;
using MUEats.Core.Domain.Events.Order;
using MUEats.Infrastructure.Persistence;
using Newtonsoft.Json;

namespace MUEats.Infrastructure.Workers;

internal sealed class FakeKitchenWorker : BackgroundService
{
    private readonly IServiceScopeFactory _serviceScopeFactory;
    private readonly ConcurrentDictionary<Guid, DateTime> _orders = new();

    public FakeKitchenWorker(IServiceScopeFactory serviceScopeFactory)
    {
        _serviceScopeFactory = serviceScopeFactory;
    }

    public bool AddOrder(Guid id) 
    {
        return _orders.TryAdd(id, DateTime.UtcNow);
    }

    protected override async Task ExecuteAsync(CancellationToken ct)
    {
        using var timer = new PeriodicTimer(TimeSpan.FromSeconds(1));
    
        while (!ct.IsCancellationRequested && await timer.WaitForNextTickAsync(ct))
        {
            var cutoff = DateTime.UtcNow.AddMinutes(-1);
        
            // Find orders ready to be processed
            var readyOrderIds = _orders
                .Where(x => x.Value <= cutoff)
                .Select(x => x.Key)
                .ToList();

            foreach (var orderId in readyOrderIds)
            {
                if (_orders.TryRemove(orderId, out _))
                {
                    try 
                    {
                        await PublishEvent(orderId, ct);
                    }
                    catch (Exception ex)
                    {
                        // If DB fails, you might want to re-add the order to the dictionary 
                        // or log it so it's not lost forever.
                        Console.WriteLine($"Error processing order {orderId}: {ex.Message}");
                    }
                }
            }
        }
    }
    
    private async Task PublishEvent(Guid orderId, CancellationToken ct)
    {
        await using var scope = _serviceScopeFactory.CreateAsyncScope();
        var context = scope.ServiceProvider.GetRequiredService<MueDbContext>();
        
        var @event = new OrderPreparedEvent { OrderId = orderId };
        var json = JsonConvert.SerializeObject(@event, JsonSerializerHelper.Settings);
        
        await context.OutboxMessages.AddAsync(new OutboxMessage
        {
            Id = Guid.NewGuid(),
            JsonPayload = json,
            Type = @event.GetType().Name,
            CreatedAt = DateTime.UtcNow,
        }, ct);
        
        await context.SaveChangesAsync(ct);
    }
}