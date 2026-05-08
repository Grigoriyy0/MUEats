using MUEats.Application.Ports;
using MUEats.Core.Domain.Order.ValueObjects;
using MUEats.Infrastructure.IntegrationEvents;

namespace MUEats.Infrastructure.Handlers;

public class OrderPreparingHandler : IIntegrationEventHandler<OrderPreparingEvent>
{
    private readonly IOrderSagaStatesRepository _orderSagaStatesRepository;
    private readonly IOrdersRepository _ordersRepository;
    private readonly IUnitOfWork _unitOfWork;

    public OrderPreparingHandler(IOrderSagaStatesRepository orderSagaStatesRepository,
        IOrdersRepository ordersRepository,
        IUnitOfWork unitOfWork)
    {
        _orderSagaStatesRepository = orderSagaStatesRepository;
        _ordersRepository = ordersRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task HandleAsync(OrderPreparingEvent message, CancellationToken ct)
    {
        await _unitOfWork.BeginTransactionAsync(ct);
        
        var sagaState = await _orderSagaStatesRepository.GetByIdAsync(message.OrderId, ct);
        var order = await _ordersRepository.GetByIdAsync(message.OrderId, ct);
        
        if (sagaState is null || order is null)
        {
            return;
        }

        if (sagaState.State >= SagaStatus.Preparing)
        {
            return;
        }

        sagaState.State = SagaStatus.Preparing;
        order.Status = OrderStatus.Preparing;

        await _unitOfWork.SaveChangesAsync(ct);
        await _unitOfWork.CommitTransactionAsync(ct);
    }
}