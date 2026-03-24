using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using MUEats.Application.Helpers;
using MUEats.Application.Ports;
using MUEats.Core;
using MUEats.Core.Domain.Events;
using MUEats.Core.Domain.Events.Order;
using MUEats.Infrastructure.Persistence;
using Newtonsoft.Json;

namespace MUEats.Infrastructure.Workers;

public class FakeRestaurantService : BackgroundService
{
    private readonly IServiceScopeFactory _scopeFactory;
    private readonly IEventBus _eventBus;

    public FakeRestaurantService(
        IServiceScopeFactory scopeFactory,
        IEventBus eventBus)
    {
        _scopeFactory = scopeFactory;
        _eventBus = eventBus;
    }

    protected override Task ExecuteAsync(CancellationToken ct)
    {
        _eventBus.Subscribe<OrderCreatedEvent>(HandleOrderCreated);
        _eventBus.Subscribe<OrderPreparingEvent>(HandleOrderPreparing);
        
        return Task.CompletedTask;
    }

    private async Task HandleOrderCreated(OrderCreatedEvent @event)
    {
        await using var scope =  _scopeFactory.CreateAsyncScope();
        
        var dbContext = scope.ServiceProvider.GetRequiredService<MueDbContext>();
        
        await Task.Delay(5000);
        
        var acceptedEvent = new OrderAcceptedEvent
        {
            OrderId = @event.OrderId
        };

        var orderStartPreparingEvent = new OrderPreparingEvent
        {
            OrderId = @event.OrderId
        };
        
        var acceptedOutboxMessage = CreateOutboxMessage(acceptedEvent);
        var startPreparingOutboxMessage = CreateOutboxMessage(orderStartPreparingEvent);
        
        await dbContext.OutboxMessages.AddRangeAsync([acceptedOutboxMessage, startPreparingOutboxMessage], CancellationToken.None);
        await dbContext.SaveChangesAsync(CancellationToken.None);
    }

    private async Task HandleOrderPreparing(OrderPreparingEvent @event)
    {
        await using var scope =  _scopeFactory.CreateAsyncScope();

        var dbContext = scope.ServiceProvider.GetRequiredService<MueDbContext>();
        
        await Task.Delay(60_000);
        
        var orderPreparedEvent = new OrderPreparedEvent
        {
            OrderId = @event.OrderId,
        };
        
        var outboxMessage = CreateOutboxMessage(orderPreparedEvent);
        
        await dbContext.OutboxMessages.AddAsync(outboxMessage, CancellationToken.None);
        await dbContext.SaveChangesAsync(CancellationToken.None);
    }
    
    private OutboxMessage CreateOutboxMessage(DomainEvent @event)
    {
        var json = JsonConvert.SerializeObject(@event, JsonSerializerHelper.Settings);

        return new OutboxMessage
        {
            Id = Guid.NewGuid(),
            CreatedAt = DateTime.UtcNow,
            JsonPayload = json,
            Type = @event.GetType().Name
        };
    }
}