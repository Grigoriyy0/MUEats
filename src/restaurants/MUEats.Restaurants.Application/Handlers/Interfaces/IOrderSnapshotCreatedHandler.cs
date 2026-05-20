namespace MUEats.Restaurants.Application.Handlers.Interfaces;

public interface IOrderSnapshotCreatedHandler
{
    Task HandleAsync(Guid snapshotId, CancellationToken ct);
}