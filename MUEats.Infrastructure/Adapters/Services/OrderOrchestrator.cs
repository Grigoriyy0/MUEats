using Microsoft.Extensions.DependencyInjection;
using MUEats.Application.Helpers;
using MUEats.Application.Ports;
using MUEats.Core;
using MUEats.Core.Domain.Events;
using MUEats.Core.Domain.Events.Courier;
using MUEats.Core.Domain.Events.Order;
using MUEats.Core.Domain.Order;
using MUEats.Core.Domain.Order.Entities;
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
        await using var scope = _scopeFactory.CreateAsyncScope();
        
        var uow  = scope.ServiceProvider.GetRequiredService<IUnitOfWork>();
        var outbox = scope.ServiceProvider.GetRequiredService<IOutboxRepository>();
        var sagaStates = scope.ServiceProvider.GetRequiredService<IOrderSagaStatesRepository>();
        
        await uow.BeginTransactionAsync(ct);

        var oderSaga = new OrderSagaState
        {
            CorrelationId = order.Id,
            State = SagaStatus.Created
        };
        
        var @event = new OrderCreatedEvent
        {
            OrderId = order.Id
        };


        var outboxMessage = CreateOutboxMessage(@event);
        
        await sagaStates.AddAsync(oderSaga, ct);
        await outbox.AddAsync(outboxMessage, ct);
        
        await uow.SaveChangesAsync(ct);
        await uow.CommitTransactionAsync(ct);

        return order.Id;
    }

    private async Task HandleOrderAccepted(OrderAcceptedEvent @event)
    {
        await using var scope = _scopeFactory.CreateAsyncScope();
        var outbox = scope.ServiceProvider.GetRequiredService<IOutboxRepository>();
        var orders = scope.ServiceProvider.GetRequiredService<IOrdersRepository>();
        var uow = scope.ServiceProvider.GetRequiredService<IUnitOfWork>();
        var sagaStates = scope.ServiceProvider.GetRequiredService<IOrderSagaStatesRepository>();
        
        await uow.BeginTransactionAsync(CancellationToken.None);
        
        //1. Check if state exists
        //2. Check if state is "Created"
        //3. Update state
        //4. Update order status
        
        var sagaState = await sagaStates.GetByIdAsync(@event.OrderId, CancellationToken.None);

        if (sagaState is null)
        {
            await Fail(@event.OrderId, "Order not found");
            return;
        }
        
        if (sagaState.State >= SagaStatus.Accepted)
        {
            //duplicate event.
            return;
        }
        
        sagaState.State =  SagaStatus.Accepted;

        
        var order = await orders.GetByIdAsync(@event.OrderId, CancellationToken.None);
        if (order is null)
        {
            await Fail(@event.OrderId, "Order not found");
            return;
        }
        
        order.OrderStatus = OrderStatus.Accepted;
        order.DeliveryStatus = DeliveryStatus.CourierRequested;

        var courierRequested = new CourierRequestedEvent
        {
            OrderId = order.Id,
            DeliveryAddress = @event.ToAddress,
            RestaurantAddress = @event.RestaurantAddress
        };
        
        var outboxMessage = CreateOutboxMessage(courierRequested);
        
        await outbox.AddAsync(outboxMessage, CancellationToken.None);
        await orders.UpdateAsync(order, CancellationToken.None);
        await sagaStates.UpdateAsync(sagaState, CancellationToken.None);
        
        await uow.SaveChangesAsync(CancellationToken.None);
        await uow.CommitTransactionAsync(CancellationToken.None);
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
        var outbox =  scope.ServiceProvider.GetRequiredService<IOutboxRepository>();
        
        await uow.BeginTransactionAsync(CancellationToken.None);
        
        var order = await orders.GetByIdAsync(@event.OrderId, CancellationToken.None);
        if (order is null)
        {
            await Fail(@event.OrderId, "Order not found");
            return;
        }

        if (order.DeliveryStatus >= DeliveryStatus.CourierFound)
            return;

        order.DeliveryStatus = DeliveryStatus.CourierFound;
        order.CourierId = @event.CourierId;
        
        await orders.UpdateAsync(order, CancellationToken.None);
        
        await uow.SaveChangesAsync(CancellationToken.None);
        await uow.CommitTransactionAsync(CancellationToken.None);
        
        if (order.OrderStatus == OrderStatus.Prepared)
        {
            await uow.BeginTransactionAsync(CancellationToken.None);
            
            var startDelivery = new DeliveryStartedEvent
            {
                OrderId = order.Id
            };
            
            var outboxMessage = CreateOutboxMessage(startDelivery);
            
            await outbox.AddAsync(outboxMessage, CancellationToken.None);
            
            await orders.UpdateAsync(order, CancellationToken.None);
            
            await uow.SaveChangesAsync(CancellationToken.None);
            await uow.CommitTransactionAsync(CancellationToken.None);
        }
    }

    private async Task HandleOrderPrepared(OrderPreparedEvent @event)
    {
        await using var scope = _scopeFactory.CreateAsyncScope();
        
        var orders = scope.ServiceProvider.GetRequiredService<IOrdersRepository>();
        var uow = scope.ServiceProvider.GetRequiredService<IUnitOfWork>();
        var outbox =  scope.ServiceProvider.GetRequiredService<IOutboxRepository>();
        var sagaStates = scope.ServiceProvider.GetRequiredService<IOrderSagaStatesRepository>();
        
        await uow.BeginTransactionAsync(CancellationToken.None);
        
        var sagaState = await sagaStates.GetByIdAsync(@event.OrderId, CancellationToken.None);

        if (sagaState is null)
        {
            await Fail(@event.OrderId, "Order not found");
            return;
        }

        if (sagaState.State >= SagaStatus.Prepared)
        {
            //duplicate event
            return;
        }
        
        sagaState.State = SagaStatus.Prepared;
        
        var order = await orders.GetByIdAsync(@event.OrderId, CancellationToken.None);
        
        if (order is null)
        {
            await Fail(@event.OrderId, "Order not found");
            return;
        }

        if (order.OrderStatus >= OrderStatus.Prepared)
        {
            return;
        }

        order.OrderStatus = OrderStatus.Prepared;
        
        await orders.UpdateAsync(order, CancellationToken.None);
        
        await uow.SaveChangesAsync(CancellationToken.None);
        await uow.CommitTransactionAsync(CancellationToken.None);

        if (order.DeliveryStatus == DeliveryStatus.CourierFound && order.OrderStatus == OrderStatus.Prepared)
        {
            await uow.BeginTransactionAsync(CancellationToken.None);
            
            var startDelivery = new DeliveryStartedEvent
            {
                OrderId = order.Id
            };
            
            var outboxMessage = CreateOutboxMessage(startDelivery);
            
            await outbox.AddAsync(outboxMessage, CancellationToken.None);
            
            await orders.UpdateAsync(order, CancellationToken.None);
            
            await uow.SaveChangesAsync(CancellationToken.None);
            await uow.CommitTransactionAsync(CancellationToken.None);
        }
    }

    private async Task HandleOrderDelivered(OrderDeliveredEvent @event)
    {
        await using var scope = _scopeFactory.CreateAsyncScope();
        var orders = scope.ServiceProvider.GetRequiredService<IOrdersRepository>();
        var uow  = scope.ServiceProvider.GetRequiredService<IUnitOfWork>();
        
        await uow.BeginTransactionAsync(CancellationToken.None);
        
        var order = await orders.GetByIdAsync(@event.OrderId, CancellationToken.None);

        if (order is null)
        {
            return;
        }

        order.OrderStatus = OrderStatus.Completed;
        order.DeliveredAt = DateTime.UtcNow;
        
        await orders.UpdateAsync(order, CancellationToken.None);
        
        await uow.SaveChangesAsync(CancellationToken.None);
        await uow.CommitTransactionAsync(CancellationToken.None);
    }

    private async Task HandleOrderFailed(OrderFailedEvent @event)
    {
        await using var scope = _scopeFactory.CreateAsyncScope();
        
        var orders = scope.ServiceProvider.GetRequiredService<IOrdersRepository>();
        var uow = scope.ServiceProvider.GetRequiredService<IUnitOfWork>();

        await uow.BeginTransactionAsync(CancellationToken.None);
        
        var order = await orders.GetByIdAsync(@event.OrderId, CancellationToken.None);
        
        if (order is null)
        {
            return;
        }

        order.OrderStatus = OrderStatus.Failed;
        
        await orders.UpdateAsync(order, CancellationToken.None);
        
        await uow.SaveChangesAsync(CancellationToken.None);
        await uow.CommitTransactionAsync(CancellationToken.None);
    }

    private async Task Fail(Guid orderId, string reason)
    {
        await using var scope = _scopeFactory.CreateAsyncScope();
        var outbox = scope.ServiceProvider.GetRequiredService<IOutboxRepository>();
        var uow = scope.ServiceProvider.GetRequiredService<IUnitOfWork>();

        await uow.BeginTransactionAsync(CancellationToken.None);
        
        var failed = new OrderFailedEvent
        {
            OrderId = orderId,
            Message = reason
        };

        var outboxMessage = CreateOutboxMessage(failed);
        
        await outbox.AddAsync(outboxMessage, CancellationToken.None);
        
        await uow.SaveChangesAsync(CancellationToken.None);
        await uow.CommitTransactionAsync(CancellationToken.None);
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