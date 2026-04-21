using MUEats.Application.Ports;
using MUEats.Core.Domain.Events.Order;
using MUEats.Core.Domain.Order.ValueObjects;

namespace MUEats.Infrastructure.Handlers;

public class OrderAcceptedHandler : IIntegrationEventHandler<OrderAcceptedEvent>
{
    private readonly IOrderSagaStatesRepository _orderSagaStatesRepository;
    private readonly IOutboxRepository _outboxRepository;
    private readonly IOrdersRepository _ordersRepository;
    private readonly IUnitOfWork _unitOfWork;

    public OrderAcceptedHandler(
        IOrderSagaStatesRepository orderSagaStatesRepository, 
        IOutboxRepository outboxRepository, 
        IOrdersRepository ordersRepository, IUnitOfWork unitOfWork)
    {
        _orderSagaStatesRepository = orderSagaStatesRepository;
        _outboxRepository = outboxRepository;
        _ordersRepository = ordersRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task HandleAsync(OrderAcceptedEvent message, CancellationToken ct)
    {
        try
        {
            await _unitOfWork.BeginTransactionAsync(ct);
            
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
            order.OrderStatus =  OrderStatus.Accepted;
        
            await _ordersRepository.UpdateAsync(order, ct);
            await _orderSagaStatesRepository.UpdateAsync(sagaState, ct);
        
            await _unitOfWork.SaveChangesAsync(ct);
            await _unitOfWork.CommitTransactionAsync(ct);
            // some notification logic here
        }
        catch (Exception)
        {
            await _unitOfWork.RollbackTransactionAsync(ct);
            throw;
        }
    }
}