using MUEats.Restaurants.Application.Ports;
using StackExchange.Redis;

namespace MUEats.Restaurants.Infrastructure.Adapters;

public class PresenceService : IPresenceService
{
    private readonly IDatabase _database;

    public PresenceService(IConnectionMultiplexer redis)
    {
        _database = redis.GetDatabase();
    }

    public async Task RegisterConnectionAsync(Guid restaurantId, string connectionId, CancellationToken ct)
    {
        ct.ThrowIfCancellationRequested();
        
        var key = $"presence:restaurantId:{restaurantId}";

        await _database.SetAddAsync(key, connectionId);
        await _database.KeyExpireAsync(key, TimeSpan.FromHours(1));
    }

    public async Task UnregisterConnectionAsync(Guid restaurantId, string connectionId, CancellationToken ct)
    {
        ct.ThrowIfCancellationRequested();
        
        var key = $"presence:restaurantId:{restaurantId}";

        await _database.SetRemoveAsync(key, connectionId);
    }

    public Task<bool> IsConnectedAsync(Guid restaurantId, CancellationToken ct)
    {
        ct.ThrowIfCancellationRequested();
        
        return _database.KeyExistsAsync($"presence:restaurantId:{restaurantId}");
    }
}