using MUEats.Application.IntegrationEvents;

namespace MUEats.Application.Ports;

public interface IEventDispatcher
{
    Task DispatchAsync(IntegrationEvent @event, CancellationToken ct);
}