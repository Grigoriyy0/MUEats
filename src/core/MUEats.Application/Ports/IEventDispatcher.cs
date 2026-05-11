using MUEats.Application.IntegrationEvents;

namespace MUEats.Application.Ports;

public interface IEventDispatcher
{
    Task DispatchAsync<T>(T @event, CancellationToken ct) where T : IntegrationEvent;
}