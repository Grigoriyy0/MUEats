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

        var saga = await _orderSagasRepository.GetByIdAsync(message.OrderId, ct);

        if (order == null || saga == null)
        {
            return;
        }

        if (saga.State != SagaState.Accepted)
        {
            return;
        }
        
        order.Status = OrderStatus.Prepared;
        
        saga.State = SagaState.Prepared;
        saga.UpdatedAt = DateTime.UtcNow;
        
        //refactor in future
        saga.PickUpDeadline = DateTime.UtcNow.AddMinutes(20);
    }
}