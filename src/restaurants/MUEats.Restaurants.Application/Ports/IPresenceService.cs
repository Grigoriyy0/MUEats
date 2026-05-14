namespace MUEats.Restaurants.Application.Ports;

public interface IPresenceService
{
    Task RegisterConnectionAsync(Guid restaurantId, string connectionId, CancellationToken ct);

    Task UnregisterConnectionAsync(Guid restaurantId, string connectionId, CancellationToken ct);

    Task<bool> IsConnectedAsync(Guid restaurantId, CancellationToken ct);
}