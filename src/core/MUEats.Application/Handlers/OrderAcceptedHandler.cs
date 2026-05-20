using MUEats.Application.IntegrationEvents;
using MUEats.Application.Ports;
using MUEats.Core.Domain.Order.ValueObjects;

namespace MUEats.Application.Handlers;

public class OrderAcceptedHandler : IIntegrationEventHandler<OrderAcceptedEvent>
{
    private readonly IOrderSagasRepository _orderSagasRepository;
    private readonly IOrdersRepository _ordersRepository;

    public OrderAcceptedHandler(IOrderSagasRepository orderSagasRepository, IOrdersRepository ordersRepository)
    {
        _orderSagasRepository = orderSagasRepository;
        _ordersRepository = ordersRepository;
    }

    public async Task HandleAsync(OrderAcceptedEvent message, CancellationToken ct)
    {
        var sagaState = await _orderSagasRepository.GetByIdAsync(message.OrderId, ct);

        var order = await _ordersRepository.GetByIdAsync(message.OrderId, ct);

        if (sagaState == null || order == null)
        {
            return;
        }

        if (sagaState.State != SagaState.Created)
        {
            return;
        }

        sagaState.State = SagaState.Accepted;
        sagaState.UpdatedAt = DateTime.UtcNow;
        
        order.Status = OrderStatus.Accepted;
    }
}