using MUEats.Restaurants.Application.Handlers.Interfaces;
using MUEats.Restaurants.Application.IntegrationEvents;
using MUEats.Restaurants.Application.Ports;
using MUEats.Restaurants.Core.Projections.Order;

namespace MUEats.Restaurants.Application.Handlers;

public class OrderSnapshotCancelHandler : IOrderSnapshotCancelHandler
{
    private readonly IUnitOfWork _uow;
    private readonly IOutboxService _outboxService;

    public OrderSnapshotCancelHandler(IUnitOfWork uow, IOutboxService outboxService)
    {
        _uow = uow;
        _outboxService = outboxService;
    }

    public async Task HandleAsync(OrderSnapshot snapshot, CancellationToken ct)
    {
        if (snapshot.Status != OrderStatus.Pending)
        {
            return;
        }   
        
        await _uow.BeginTransactionAsync(ct);

        snapshot.LockId = null;
        snapshot.Status = OrderStatus.Rejected;

        var @event = new OrderRejectedEvent
        {
            OrderId = snapshot.OrderId,
            Reason = "timeout"
        };

        await _outboxService.AddAsync(@event, ct);

        await _uow.SaveChangesAsync(ct);
        await _uow.CommitTransactionAsync(ct);
    }
}