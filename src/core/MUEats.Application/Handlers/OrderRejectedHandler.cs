using MUEats.Application.IntegrationEvents;
using MUEats.Application.Ports;
using MUEats.Core.Domain.Order.ValueObjects;

namespace MUEats.Application.Handlers;

public class OrderRejectedHandler : IIntegrationEventHandler<OrderRejectedEvent>
{
    private readonly IOrderSagasRepository _orderSagasRepository;
    private readonly IOrdersRepository _ordersRepository;

    public OrderRejectedHandler(IOrderSagasRepository orderSagasRepository, IOrdersRepository ordersRepository)
    {
        _orderSagasRepository = orderSagasRepository;
        _ordersRepository = ordersRepository;
    }

    public async Task HandleAsync(OrderRejectedEvent message, CancellationToken ct)
    {
        var order = await _ordersRepository.GetByIdAsync(message.OrderId, ct);
        var saga = await _orderSagasRepository.GetByIdAsync(message.OrderId, ct);

        if (order == null || saga == null)
        {
            return;
        }

        if (saga.State != SagaState.Created)
        {
            return;
        }
        
        saga.State = SagaState.Rejected;
        saga.UpdatedAt = DateTime.UtcNow;
        
        order.Status = OrderStatus.Rejected;
        order.RejectReason = message.Reason;
    }
}