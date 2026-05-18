using Microsoft.Extensions.Logging;
using MUEats.Restaurants.Application.Handlers.Interfaces;
using MUEats.Restaurants.Application.IntegrationEvents;
using MUEats.Restaurants.Application.Ports;
using MUEats.Restaurants.Core.Projections.Order;

namespace MUEats.Restaurants.Application.Handlers;

public class OrderSnapshotCancelHandler : IOrderSnapshotCancelHandler
{
    private readonly IUnitOfWork _uow;
    private readonly IOutboxService _outboxService;
    private readonly IOrderSnapshotsRepository _repository;
    private readonly ILogger<OrderSnapshotCancelHandler> _logger;
    
    public OrderSnapshotCancelHandler(IUnitOfWork uow, 
        IOutboxService outboxService, 
        IOrderSnapshotsRepository repository,
        ILogger<OrderSnapshotCancelHandler> logger)
    {
        _uow = uow;
        _outboxService = outboxService;
        _repository = repository;
        _logger = logger;
    }

    public async Task HandleAsync(Guid snapshotId, CancellationToken ct)
    {
        
        await _uow.BeginTransactionAsync(ct);

        var snapshot = await _repository.GetByIdAsync(snapshotId, ct);
        
        if (snapshot is null || snapshot.Status != OrderStatus.Pending)
        {
            await _uow.RollbackTransactionAsync(ct);
            return;
        }   
        
        _logger.LogTrace("Started cancellation of order {0} because of timeout", snapshot.OrderId);

        
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