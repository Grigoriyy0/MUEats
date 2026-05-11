using MUEats.Application.IntegrationEvents;
using MUEats.Application.Ports;
using MUEats.Core.Domain.Order.ValueObjects;

namespace MUEats.Application.Handlers;

public class OrderPreparedHandler : IIntegrationEventHandler<OrderPreparedEvent>
{
    private readonly IOrderSagasRepository _orderSagasRepository;
    private readonly IOrdersRepository _ordersRepository;

    public OrderPreparedHandler(IOrderSagasRepository orderSagasRepository, IOrdersRepository ordersRepository)
    {
        _orderSagasRepository = orderSagasRepository;
        _ordersRepository = ordersRepository;
    }

    public async Task HandleAsync(OrderPreparedEvent message, CancellationToken ct)
    {
        var order = await _ordersRepository.GetByIdAsync(message.OrderId, ct);

        var state = await _orderSagasRepository.GetByIdAsync(message.OrderId, ct);

        if (order == null || state == null)
        {
            return;
        }
        
        order.Status = OrderStatus.Prepared;
        
        state.State = SagaState.Prepared;
        state.UpdatedAt = DateTime.UtcNow;
        
        //refactor in future
        state.PickUpDeadline = DateTime.UtcNow.AddMinutes(20);
    }
}