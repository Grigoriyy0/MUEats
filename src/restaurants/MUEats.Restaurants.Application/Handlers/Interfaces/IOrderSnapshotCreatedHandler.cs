using MUEats.Restaurants.Core.Projections.Order;

namespace MUEats.Restaurants.Infrastructure.Handlers.Interfaces;

public interface IOrderSnapshotCreatedHandler
{
    Task HandleAsync(OrderSnapshot snapshot, CancellationToken ct);
}