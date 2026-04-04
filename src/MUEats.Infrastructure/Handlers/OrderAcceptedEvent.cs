using MUEats.Application.Ports;

namespace MUEats.Infrastructure.Handlers;

public class OrderAcceptedEvent : IIntegrationEventHandler<OrderAcceptedEvent>
{
    private readonly IOrderSagaStatesRepository _orderSagaStatesRepository;
    private readonly IOutboxRepository _outboxRepository;
    private readonly IOrdersRepository _ordersRepository;

    public OrderAcceptedEvent(
        IOrderSagaStatesRepository orderSagaStatesRepository, 
        IOutboxRepository outboxRepository, 
        IOrdersRepository ordersRepository)
    {
        _orderSagaStatesRepository = orderSagaStatesRepository;
        _outboxRepository = outboxRepository;
        _ordersRepository = ordersRepository;
    }

    public async Task HandleAsync(OrderAcceptedEvent message, CancellationToken ct)
    {
        throw new NotImplementedException();
    }
}