namespace MUEats.Restaurants.Application.Ports;

public interface IPresenceService
{
    Task RegisterConnectionAsync(Guid restaurantId, string connectionId);

    Task UnregisterConnectionAsync(Guid restaurantId, string connectionId);

    Task<bool> IsConnected(Guid restaurantId);
}