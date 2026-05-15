using MUEats.Restaurants.Application.DTOs;

namespace MUEats.Restaurants.Application.Ports;

public interface IRealtimeDispatcher
{
    Task DispatchAsync(Guid restaurantId, OrderDto dto, CancellationToken ct);
}