using MUEats.Application.IntegrationEvents;
using MUEats.Application.Ports;
using MUEats.Core.Domain.Order.ValueObjects;

namespace MUEats.Application.Handlers;

public class OrderAcceptedHandler : IIntegrationEventHandler<OrderAcceptedEvent>
{
    private readonly IOrderSagaStatesRepository _orderSagaStatesRepository;
    private readonly IOrdersRepository _ordersRepository;

    public OrderAcceptedHandler(
        IOrderSagaStatesRepository orderSagaStatesRepository,
        IOrdersRepository ordersRepository)
    {
        _orderSagaStatesRepository = orderSagaStatesRepository;
        _ordersRepository = ordersRepository;
    }

    public async Task HandleAsync(OrderAcceptedEvent message, CancellationToken ct)
    {
        var sagaState = await _orderSagaStatesRepository.GetByIdAsync(message.OrderId, ct);

        var order = await _ordersRepository.GetByIdAsync(message.OrderId, ct);

        if (sagaState == null || order == null)
        {
            return;
        }

        if (sagaState.State > SagaStatus.WaitingForApproval)
        {
            return;
        }

        sagaState.State = SagaStatus.Accepted;
        order.Status = OrderStatus.Accepted;
    }
}