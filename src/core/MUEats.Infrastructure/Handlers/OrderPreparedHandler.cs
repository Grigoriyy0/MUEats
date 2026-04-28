using MUEats.Application.Ports;
using MUEats.Core.Domain.Events.Order;
using MUEats.Core.Domain.Order.ValueObjects;

namespace MUEats.Infrastructure.Handlers;

public class OrderPreparedHandler : IIntegrationEventHandler<OrderPreparedEvent>
{
    private readonly IOrderSagaStatesRepository _orderSagaStatesRepository;
    private readonly IOrdersRepository _ordersRepository;
    private readonly IUnitOfWork _unitOfWork;

    public OrderPreparedHandler(IOrderSagaStatesRepository orderSagaStatesRepository, 
        IOrdersRepository ordersRepository, 
        IUnitOfWork unitOfWork)
    {
        _orderSagaStatesRepository = orderSagaStatesRepository;
        _ordersRepository = ordersRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task HandleAsync(OrderPreparedEvent message, CancellationToken ct)
    {
        await _unitOfWork.BeginTransactionAsync(ct);
        
        var order = await _ordersRepository.GetByIdAsync(message.OrderId, ct);
        
        var state = await _orderSagaStatesRepository.GetByIdAsync(message.OrderId, ct);

        if (order == null)
        {
            return;
        }

        if (state == null)
        {
            return;
        }

        order.OrderStatus = OrderStatus.Prepared;
        state.State = SagaStatus.Prepared;
        
        await _ordersRepository.UpdateAsync(order, ct);
        await _orderSagaStatesRepository.UpdateAsync(state, ct);
        
        await _unitOfWork.SaveChangesAsync(ct);
        await _unitOfWork.CommitTransactionAsync(ct);
    }
}