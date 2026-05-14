using MUEats.Restaurants.Infrastructure.ExternalServices.Api;

namespace MUEats.Restaurants.Infrastructure.Handlers.Interfaces;

public interface IOrderSnapshotCreatedHandler
{
    Task HandleAsync(OrderSnapshot snapshot, CancellationToken ct);
}