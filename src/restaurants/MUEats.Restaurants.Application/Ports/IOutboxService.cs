using MUEats.Restaurants.Application.IntegrationEvents;

namespace MUEats.Restaurants.Application.Ports;

public interface IOutboxService
{
    Task AddAsync(IntegrationEvent @event, CancellationToken ct);
}