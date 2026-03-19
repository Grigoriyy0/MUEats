using Microsoft.Extensions.DependencyInjection;
using MUEats.Application.Helpers;
using MUEats.Application.Ports;
using MUEats.Core;
using MUEats.Core.Domain.Events.Courier;
using MUEats.Core.Domain.Events.Order;
using MUEats.Core.Domain.Order;
using Newtonsoft.Json;

namespace MUEats.Infrastructure.Adapters.Services;

public class OrderSaga
{
    private readonly IServiceScopeFactory _serviceScopeFactory;
    private readonly IEventBus _eventBus;

    public OrderSaga(IEventBus eventBus, IServiceScopeFactory serviceScopeFactory)
    {
        _eventBus = eventBus;
        _serviceScopeFactory = serviceScopeFactory;

        _eventBus.Subscribe<OrderCreatedEvent>(HandleOrderCreated);
        _eventBus.Subscribe<OrderAcceptedEvent>(HandleOrderAccepted);
        _eventBus.Subscribe<OrderPreparedEvent>(HandleOrderPreparedEvent);
        _eventBus.Subscribe<OrderFailedEvent>(HandleOrderFailedEvent);

        _eventBus.Subscribe<CourierRequestedEvent>(HandleCourierRequested);
        _eventBus.Subscribe<CourierFoundEvent>(HandleCourierFound);
    }

    public async Task<Guid> StartOrderSagaAsync(Order order, CancellationToken ct)
    {
        await using var scope = _serviceScopeFactory.CreateAsyncScope();

        var repo = scope.ServiceProvider.GetRequiredService<IOutboxRepository>();
        
        var @event = new OrderCreatedEvent
        {
            OrderId = order.Id
        };

        var json = JsonConvert.SerializeObject(@event, JsonSerializerHelper.Settings);

        var outboxMessage = new OutboxMessage
        {
            Id = Guid.NewGuid(),
            CreatedAt = DateTime.UtcNow,
            JsonPayload = json,
            Type = @event.GetType().Name,
        };

        await repo.AddAsync(outboxMessage, ct);

        return @event.OrderId;
    }
    
    private async Task HandleOrderCreated(OrderCreatedEvent @event)
    {
        await using var scope = _serviceScopeFactory.CreateAsyncScope();

        var restaurantProvider = scope.ServiceProvider.GetRequiredService<IRestaurantsProvider>();
        var outboxRepository = scope.ServiceProvider.GetRequiredService<IOutboxRepository>();
        var orderRepository = scope.ServiceProvider.GetRequiredService<IOrdersRepository>();

        var orderDto = await orderRepository.GetDtoByIdAsync(@event.OrderId, CancellationToken.None);

        if (orderDto is null)
        {
            return;
        }

        var result = await restaurantProvider.SubmitOrderAsync(orderDto, CancellationToken.None);

        if (!result)
        {
            await FailOrder(orderDto.Id, "Restaurant.rejection");
        }
        
        var orderAcceptedEvent = new OrderAcceptedEvent
        {
            OrderId = orderDto.Id,
            RestaurantAddress = orderDto.RestaurantDetails,
            ToAddress = orderDto.DeliveryAddress,
            DeliveryReward = orderDto.TotalPrice
        };
        
        var json = JsonConvert.SerializeObject(orderAcceptedEvent, JsonSerializerHelper.Settings);

        var outboxMessage = new OutboxMessage
        {
            Id = Guid.NewGuid(),
            CreatedAt = DateTime.UtcNow,
            JsonPayload = json,
            Type = orderAcceptedEvent.GetType().Name,
        };

        await outboxRepository.AddAsync(outboxMessage, CancellationToken.None);
    }

    private async Task HandleOrderAccepted(OrderAcceptedEvent @event)
    {
        await using var scope = _serviceScopeFactory.CreateAsyncScope();

        var orderRepository = scope.ServiceProvider.GetRequiredService<IOrdersRepository>();

        var order = await orderRepository.GetByIdAsync(@event.OrderId, CancellationToken.None);

        if (order is null)
        {
            await FailOrder(@event.OrderId, "Not found");
        }
        
        
    }

    private async Task HandleCourierRequested(CourierRequestedEvent @event)
    {
        
    }

    private async Task HandleCourierFound(CourierFoundEvent @event)
    {
        
    }

    private async Task HandleOrderPreparedEvent(OrderPreparedEvent @event)
    {
        
    }

    private async Task HandleOrderFailedEvent(OrderFailedEvent @event)
    {
        
    }
    
    private Task FailOrder(Guid orderId, string msg)
    {
        using var scope = _serviceScopeFactory.CreateAsyncScope();

        var outboxRepository = scope.ServiceProvider.GetRequiredService<IOutboxRepository>();
        
        var failed = new OrderFailedEvent
        {
            OrderId = orderId,
            Message = msg
        };
            
        var jsonFailed = JsonConvert.SerializeObject(failed, JsonSerializerHelper.Settings);


        var outboxMessage = new OutboxMessage
        {
            Id = Guid.NewGuid(),
            CreatedAt = DateTime.UtcNow,
            JsonPayload = jsonFailed,
            Type = failed.GetType().Name
        };

        return outboxRepository.AddAsync(outboxMessage, CancellationToken.None);
    }
}