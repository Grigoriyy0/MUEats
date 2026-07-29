using IntegrationEvent = SharedContracts.IntegrationEvents.IntegrationEvent;

namespace MUEats.Restaurants.Application.Ports;

public interface IOutboxService
{
    Task AddAsync(IntegrationEvent @event, CancellationToken ct);
}