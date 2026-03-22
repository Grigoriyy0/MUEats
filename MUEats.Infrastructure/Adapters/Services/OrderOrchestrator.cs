using Microsoft.Extensions.DependencyInjection;
using MUEats.Application.Helpers;
using MUEats.Application.Ports;
using MUEats.Core;
using MUEats.Core.Domain.Events;
using MUEats.Core.Domain.Events.Courier;
using MUEats.Core.Domain.Events.Order;
using MUEats.Core.Domain.Order;
using MUEats.Core.Domain.Order.ValueObjects;
using Newtonsoft.Json;

namespace MUEats.Infrastructure.Adapters.Services;

public class OrderOrchestrator : IOrderOrchestrator
{
    private readonly IEventBus _eventBus;
    private readonly IServiceScopeFactory _scopeFactory;

    public OrderOrchestrator(
        IServiceScopeFactory scopeFactory,
        IEventBus eventBus)
    {
        _scopeFactory = scopeFactory;
        _eventBus = eventBus;

        _eventBus.Subscribe<OrderAcceptedEvent>(HandleOrderAccepted);
        _eventBus.Subscribe<OrderPreparedEvent>(HandleOrderPrepared);
        _eventBus.Subscribe<CourierRequestedEvent>(HandleCourierRequested);
        _eventBus.Subscribe<CourierFoundEvent>(HandleCourierFound);
        _eventBus.Subscribe<OrderFailedEvent>(HandleOrderFailed);
        _eventBus.Subscribe<OrderDeliveredEvent>(HandleOrderDelivered);
    }

    public async Task<Guid> StartAsync(Order order, CancellationToken ct)
    {
        var @event = new OrderCreatedEvent
        {
            OrderId = order.Id
        };

        await Save(@event, ct);

        return order.Id;
    }

    private async Task HandleOrderAccepted(OrderAcceptedEvent @event)
    {
        await using var scope = _scopeFactory.CreateAsyncScope();
        var outbox = scope.ServiceProvider.GetRequiredService<IOutboxRepository>();
        var orders = scope.ServiceProvider.GetRequiredService<IOrdersRepository>();
        var uow = scope.ServiceProvider.GetRequiredService<IUnitOfWork>();

        var order = await orders.GetByIdAsync(@event.OrderId, CancellationToken.None);
        if (order is null)
        {
            await Fail(@event.OrderId, "Order not found");
            return;
        }

        if (order.Status >= OrderStatus.Accepted)
            return;

        order.Status = OrderStatus.Accepted;

        var courierRequested = new CourierRequestedEvent
        {
            OrderId = order.Id,
            DeliveryAddress = @event.ToAddress,
            RestaurantAddress = @event.RestaurantAddress
        };

        await Save(courierRequested);
    }

    private async Task HandleCourierRequested(CourierRequestedEvent @event)
    {
        await Task.CompletedTask;
    }

    private async Task HandleCourierFound(CourierFoundEvent @event)
    {
        await using var scope = _scopeFactory.CreateAsyncScope();
        var orders = scope.ServiceProvider.GetRequiredService<IOrdersRepository>();
        var uow = scope.ServiceProvider.GetRequiredService<IUnitOfWork>();

        var order = await orders.GetByIdAsync(@event.OrderId, CancellationToken.None);
        if (order is null)
        {
            await Fail(@event.OrderId, "Order not found");
            return;
        }

        if (order.Status >= OrderStatus.CourierFound)
            return;

        order.Status = OrderStatus.CourierFound;
    }

    private async Task HandleOrderPrepared(OrderPreparedEvent @event)
    {
        await using var scope = _scopeFactory.CreateAsyncScope();
        var orders = scope.ServiceProvider.GetRequiredService<IOrdersRepository>();

        var order = await orders.GetByIdAsync(@event.OrderId, CancellationToken.None);
        if (order is null)
        {
            await Fail(@event.OrderId, "Order not found");
            return;
        }

        if (order.Status >= OrderStatus.Prepared)
            return;

        order.Status = OrderStatus.Prepared;

        if (order.Status == OrderStatus.CourierFound || order.Status == OrderStatus.Prepared)
        {
            var startDelivery = new DeliveryStartedEvent
            {
                OrderId = order.Id
            };

            await Save(startDelivery);
        }
    }

    private async Task HandleOrderDelivered(OrderDeliveredEvent @event)
    {
        await using var scope = _scopeFactory.CreateAsyncScope();
        var orders = scope.ServiceProvider.GetRequiredService<IOrdersRepository>();
        
        var order = await orders.GetByIdAsync(@event.OrderId, CancellationToken.None);

        if (order is null)
        {
            return;
        }

        order.Status = OrderStatus.Completed;

        await SaveChanges(order);
    }

    private async Task HandleOrderFailed(OrderFailedEvent @event)
    {
        await using var scope = _scopeFactory.CreateAsyncScope();
        var orders = scope.ServiceProvider.GetRequiredService<IOrdersRepository>();
        var uow = scope.ServiceProvider.GetRequiredService<IUnitOfWork>();

        var order = await orders.GetByIdAsync(@event.OrderId, CancellationToken.None);
        if (order is null)
            return;

        order.Status = OrderStatus.Failed;
    }

    private async Task Fail(Guid orderId, string reason)
    {
        await using var scope = _scopeFactory.CreateAsyncScope();
        var outbox = scope.ServiceProvider.GetRequiredService<IOutboxRepository>();
        var uow = scope.ServiceProvider.GetRequiredService<IUnitOfWork>();

        var failed = new OrderFailedEvent
        {
            OrderId = orderId,
            Message = reason
        };

        await Save(failed);
    }

    private async Task Save(DomainEvent @event, CancellationToken ct = default)
    {
        await using var scope = _scopeFactory.CreateAsyncScope();
        var uow = scope.ServiceProvider.GetRequiredService<IUnitOfWork>();
        var outbox = scope.ServiceProvider.GetRequiredService<IOutboxRepository>();
        
        
        var json = JsonConvert.SerializeObject(@event, JsonSerializerHelper.Settings);

        await uow.BeginTransactionAsync(ct);
        
        var message = new OutboxMessage
        {
            Id = Guid.NewGuid(),
            CreatedAt = DateTime.UtcNow,
            JsonPayload = json,
            Type = @event.GetType().Name
        };

        await outbox.AddAsync(message, ct);
        
        await uow.SaveChangesAsync(ct);
        await uow.CommitTransactionAsync(ct);
    }

    private async Task SaveChanges(Order order, CancellationToken ct = default)
    {
        await using var scope = _scopeFactory.CreateAsyncScope();
        var uow = scope.ServiceProvider.GetRequiredService<IUnitOfWork>();
        var orders = scope.ServiceProvider.GetRequiredService<IOrdersRepository>();
        
        await uow.BeginTransactionAsync(ct);
        
        await orders.UpdateAsync(order, ct);
        
        await uow.SaveChangesAsync(ct);
        await uow.CommitTransactionAsync(ct);
    }
}