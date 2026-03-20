using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using MUEats.Application.Helpers;
using MUEats.Application.Ports;
using MUEats.Core;
using MUEats.Core.Domain.Events;
using MUEats.Core.Domain.Events.Courier;
using Newtonsoft.Json;

namespace MUEats.Infrastructure.Workers;

public class FakeCourierService : BackgroundService
{
    private readonly IServiceScopeFactory _scopeFactory;
    private readonly IEventBus _eventBus;
    private readonly Random _random = new();

    public FakeCourierService(
        IServiceScopeFactory scopeFactory,
        IEventBus eventBus)
    {
        _scopeFactory = scopeFactory;
        _eventBus = eventBus;
    }

    protected override Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _eventBus.Subscribe<CourierRequestedEvent>(Handle);

        return Task.CompletedTask;
    }

    private async Task Handle(CourierRequestedEvent @event)
    {
        var delay = _random.Next(30_000, 180_000); // 30 сек – 3 мин
        await Task.Delay(5000);

        await using var scope = _scopeFactory.CreateAsyncScope();

        var outbox = scope.ServiceProvider.GetRequiredService<IOutboxRepository>();

        // шанс не найти курьера
        var found = _random.NextDouble() > 0.05;

        /*if (!found)
        {
            var failed = new С
            {
                OrderId = @event.OrderId,
                Reason = "No couriers available"
            };

            await Save(outbox, failed);
            return;
        } */

        var courierFound = new CourierFoundEvent
        {
            OrderId = @event.OrderId,
            CourierId = Guid.NewGuid(),
            EstimatedArrival = DateTime.UtcNow.AddMinutes(_random.Next(5, 15))
        };

        await Save(courierFound);
    }

    private async Task Save(DomainEvent @event)
    {
        await using var scope = _scopeFactory.CreateAsyncScope();
        
        var outbox = scope.ServiceProvider.GetRequiredService<IOutboxRepository>();
        var uow = scope.ServiceProvider.GetRequiredService<IUnitOfWork>();
        
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