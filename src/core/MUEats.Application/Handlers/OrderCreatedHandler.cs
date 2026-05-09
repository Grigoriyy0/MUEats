using MUEats.Application.IntegrationEvents;
using MUEats.Application.Ports;
using MUEats.Core.Domain.Order.ValueObjects;

namespace MUEats.Application.Handlers;

public class OrderCreatedHandler : IIntegrationEventHandler<OrderCreatedEvent>
{
    private readonly IOrderSagaStatesRepository _orderSagaStatesRepository;
    private readonly IOrdersQueries _ordersQueries;

    public OrderCreatedHandler(IOrderSagaStatesRepository orderSagaStatesRepository,
        IOrdersQueries ordersQueries)
    {
        _orderSagaStatesRepository = orderSagaStatesRepository;
        _ordersQueries = ordersQueries;
    }

    //todo add create sagaState
    public async Task HandleAsync(OrderCreatedEvent message, CancellationToken ct)
    {
        var order = await _ordersQueries.GetDtoByIdAsync(message.OrderId, ct);
        var sagaState = await _orderSagaStatesRepository.GetByIdAsync(message.OrderId, ct);

        if (order == null) return;

        sagaState.State = SagaStatus.WaitingForApproval;

        await _orderSagaStatesRepository.AddAsync(sagaState, ct);
    }
}