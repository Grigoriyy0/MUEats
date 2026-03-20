using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using MUEats.Application.Helpers;
using MUEats.Application.Ports;
using MUEats.Core;
using MUEats.Core.Domain.Events;
using MUEats.Core.Domain.Events.Order;
using Newtonsoft.Json;

namespace MUEats.Infrastructure.Workers;

public class FakeRestaurantService : BackgroundService
{
    private readonly IServiceScopeFactory _scopeFactory;
    private readonly IEventBus _eventBus;
    private readonly Random _random = new();

    public FakeRestaurantService(
        IServiceScopeFactory scopeFactory,
        IEventBus eventBus)
    {
        _scopeFactory = scopeFactory;
        _eventBus = eventBus;
    }

    protected override Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _eventBus.Subscribe<OrderCreatedEvent>(Handle);

        return Task.CompletedTask;
    }

    private async Task Handle(OrderCreatedEvent @event)
    {
        var delay = _random.Next(60_000, 300_000);
        await Task.Delay(5000);

        await using var scope = _scopeFactory.CreateAsyncScope();
        
        var accepted = _random.NextDouble() > 0.0;

        if (!accepted)
        {
            var failed = new OrderFailedEvent
            {
                OrderId = @event.OrderId,
                Message = "Restaurant rejected"
            };

            await Save(failed);
            return;
        }

        var acceptedEvent = new OrderAcceptedEvent
        {
            OrderId = @event.OrderId,
            //RestaurantAddress = @event.,
            //ToAddress = @event.ToAddress,
            //DeliveryReward = @event.DeliveryReward
        };

        await Save(acceptedEvent);
    }

    private async Task Save(DomainEvent @event)
    {
        await using var scope = _scopeFactory.CreateAsyncScope();
        
        var outbox = scope.ServiceProvider.GetRequiredService<IOutboxRepository>();
        var uow =  scope.ServiceProvider.GetRequiredService<IUnitOfWork>();
        
        await uow.BeginTransactionAsync(CancellationToken.None);
        
        var json = JsonConvert.SerializeObject(@event, JsonSerializerHelper.Settings);

        var message = new OutboxMessage
        {
            Id = Guid.NewGuid(),
            CreatedAt = DateTime.UtcNow,
            JsonPayload = json,
            Type = @event.GetType().Name
        };

        await outbox.AddAsync(message, CancellationToken.None);
        
        await uow.SaveChangesAsync(CancellationToken.None);
        await uow.CommitTransactionAsync(CancellationToken.None);
    }
}