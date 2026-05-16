using MUEats.Restaurants.Application.DTOs;
using MUEats.Restaurants.Core.Projections.Order;

namespace MUEats.Restaurants.Application.Ports;

public interface IOrderSnapshotsRepository
{
    Task<List<OrderDto>> GetPendingAsync(Guid restaurantId, CancellationToken ct);

    Task<OrderSnapshot?> GetByIdAsync(Guid orderId, CancellationToken ct);
}