using MUEats.Application.IntegrationEvents;

namespace MUEats.Application.Ports;

public interface IOutboxService
{
    Task CreateAsync(IntegrationEvent @event, CancellationToken ct);
}