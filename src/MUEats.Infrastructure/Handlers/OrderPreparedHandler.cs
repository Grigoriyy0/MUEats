using MUEats.Core.Domain.Events.Order;

namespace MUEats.Infrastructure.Handlers;

public class OrderPreparedHandler : IIntegrationEventHandler<OrderPreparedEvent>
{
    public async Task HandleAsync(OrderPreparedEvent message, CancellationToken ct)
    {
        throw new NotImplementedException();
    }
}