using MUEats.Application.IntegrationEvents;
using MUEats.Application.Ports;
using MUEats.Core.Domain.Order.ValueObjects;

namespace MUEats.Application.Handlers;

public class OrderCancelledHandler : IIntegrationEventHandler<OrderCancelledEvent>
{
    private  readonly IOrderSagasRepository _orderSagasRepository;
    private readonly IOrdersRepository _ordersRepository;

    public OrderCancelledHandler(IOrderSagasRepository orderSagasRepository, IOrdersRepository ordersRepository)
    {
        _orderSagasRepository = orderSagasRepository;
        _ordersRepository = ordersRepository;
    }

    public async Task HandleAsync(OrderCancelledEvent message, CancellationToken ct)
    {
        var order = await _ordersRepository.GetByIdAsync(message.OrderId, ct);
        var saga = await _orderSagasRepository.GetByIdAsync(message.OrderId, ct);

        if (order == null || saga == null)
        {
            return;
        }

        if (saga.State == SagaState.Completed || saga.State == SagaState.Rejected || saga.State == SagaState.Cancelled)
        {
            return;
        }
        
        saga.State = SagaState.Cancelled;
        saga.UpdatedAt = DateTime.UtcNow;
        order.Status = OrderStatus.Cancelled;
        
        //todo think about compensation policies, e.g. if order == accepted, if order == prepared
    }
}