using MUEats.Application.Helpers;
using MUEats.Application.Ports;
using MUEats.Core;
using MUEats.Core.Domain.Events.Order;
using MUEats.Core.Domain.Order.Entities;
using MUEats.Core.Domain.Order.ValueObjects;
using Newtonsoft.Json;

namespace MUEats.Infrastructure.Handlers;

public class OrderCreatedHandler : IIntegrationEventHandler<OrderCreatedEvent>
{
    private readonly IOrderSagaStatesRepository _orderSagaStatesRepository;
    private readonly IOutboxRepository _outboxRepository;
    private readonly IOrdersRepository _ordersRepository;

    public OrderCreatedHandler(
        IOrderSagaStatesRepository orderSagaStatesRepository, 
        IOutboxRepository outboxRepository, 
        IOrdersRepository ordersRepository
        )
    {
        _orderSagaStatesRepository = orderSagaStatesRepository;
        _outboxRepository = outboxRepository;
        _ordersRepository = ordersRepository;
    }

    public async Task HandleAsync(OrderCreatedEvent message, CancellationToken ct)
    {
        var order = await _ordersRepository.GetDtoByIdAsync(message.OrderId, ct);

        if (order == null)
        {
            return;
        }
        
        var sagaState = new OrderSagaState
        {
            CorrelationId = message.Id,
            State = SagaStatus.Created
        };

        var @event = new OrderSentEvent
        {
            OrderId = order.Id,
            RestaurantId = order.RestaurantId,
        };
        
        var json = JsonConvert.SerializeObject(@event, JsonSerializerHelper.Settings);

        var outboxMessage = new OutboxMessage
        {
            Id = Guid.NewGuid(),
            CreatedAt = DateTime.UtcNow,
            JsonPayload = json,
            Type = @event.GetType().Name,
        };
        
        await _orderSagaStatesRepository.AddAsync(sagaState, ct);
        await _outboxRepository.AddAsync(outboxMessage, ct);
    }
}