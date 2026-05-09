using MUEats.Application.IntegrationEvents;
using MUEats.Application.Ports;
using MUEats.Core.Domain.Order.ValueObjects;

namespace MUEats.Application.Handlers;

public class OrderPreparedHandler : IIntegrationEventHandler<OrderPreparedEvent>
{
    private readonly IOrderSagaStatesRepository _orderSagaStatesRepository;
    private readonly IOrdersRepository _ordersRepository;

    public OrderPreparedHandler(IOrderSagaStatesRepository orderSagaStatesRepository,
        IOrdersRepository ordersRepository)
    {
        _orderSagaStatesRepository = orderSagaStatesRepository;
        _ordersRepository = ordersRepository;
    }

    public async Task HandleAsync(OrderPreparedEvent message, CancellationToken ct)
    {
        var order = await _ordersRepository.GetByIdAsync(message.OrderId, ct);

        var state = await _orderSagaStatesRepository.GetByIdAsync(message.OrderId, ct);

        if (order == null || state == null)
        {
            return;
        }
        
        order.Status = OrderStatus.Prepared;
        state.State = SagaStatus.Prepared;
    }
}