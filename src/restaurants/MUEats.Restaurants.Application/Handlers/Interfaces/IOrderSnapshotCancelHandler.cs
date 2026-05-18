namespace MUEats.Restaurants.Application.Handlers.Interfaces;

public interface IOrderSnapshotCancelHandler
{
    Task HandleAsync(Guid snapshotId, CancellationToken ct);
}