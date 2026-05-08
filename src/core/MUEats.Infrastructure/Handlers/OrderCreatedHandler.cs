using MUEats.Application.Helpers;
using MUEats.Application.Ports;
using MUEats.Core;
using MUEats.Core.Domain.Events.Order;
using MUEats.Core.Domain.Order.ValueObjects;
using Newtonsoft.Json;

namespace MUEats.Infrastructure.Handlers;

public class OrderCreatedHandler : IIntegrationEventHandler<OrderCreatedEvent>
{
    private readonly IOrderSagaStatesRepository _orderSagaStatesRepository;
    private readonly IOutboxService _outboxService;
    private readonly IOrdersQueries _ordersQueries;
    private readonly IUnitOfWork _unitOfWork;

    public OrderCreatedHandler(IOrderSagaStatesRepository orderSagaStatesRepository, 
        IUnitOfWork unitOfWork, 
        IOrdersQueries ordersQueries, 
        IOutboxService outboxService)
    {
        _orderSagaStatesRepository = orderSagaStatesRepository;
        _unitOfWork = unitOfWork;
        _ordersQueries = ordersQueries;
        _outboxService = outboxService;
    }

    //todo add create sagaState
    public async Task HandleAsync(OrderCreatedEvent message, CancellationToken ct)
    {
        try
        {
            await _unitOfWork.BeginTransactionAsync(ct);
            
            var order = await _ordersQueries.GetDtoByIdAsync(message.OrderId, ct);
            var sagaState = await _orderSagaStatesRepository.GetByIdAsync(message.OrderId, ct);
            
            if (order == null)
            {
                return;
            }
            
            var @event = new OrderSentEvent
            {
                OrderId = order.Id,
                RestaurantId = order.RestaurantId,
            };

            sagaState.State = SagaStatus.WaitingForApproval;
            
            await _orderSagaStatesRepository.AddAsync(sagaState, ct);
            await _outboxService.CreateAsync(@event, ct);
            
            await _unitOfWork.SaveChangesAsync(ct);
            await _unitOfWork.CommitTransactionAsync(ct);
        }
        catch (Exception)
        {
            await _unitOfWork.RollbackTransactionAsync(ct);
            throw;
        }
    }
}