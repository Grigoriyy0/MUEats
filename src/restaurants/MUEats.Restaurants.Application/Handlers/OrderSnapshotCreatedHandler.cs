using MUEats.Restaurants.Application.DTOs;
using MUEats.Restaurants.Application.Handlers.Interfaces;
using MUEats.Restaurants.Application.IntegrationEvents;
using MUEats.Restaurants.Application.Ports;
using MUEats.Restaurants.Core.Projections.Order;

namespace MUEats.Restaurants.Application.Handlers;

public class OrderSnapshotCreatedHandler : IOrderSnapshotCreatedHandler
{
    private readonly IRealtimeDispatcher _dispatcher;
    private readonly IPresenceService _presenceService;
    private readonly IOutboxService _outboxService;
    private readonly IUnitOfWork _uow;
    private readonly IOrderSnapshotsRepository _repository;
    
    private const int MaxRetryCount = 3;
    
    public OrderSnapshotCreatedHandler(IRealtimeDispatcher dispatcher, 
        IPresenceService presenceService, 
        IOutboxService outboxService, 
        IUnitOfWork uow, IOrderSnapshotsRepository repository)
    {
        _dispatcher = dispatcher;
        _presenceService = presenceService;
        _outboxService = outboxService;
        _uow = uow;
        _repository = repository;
    }

    public async Task HandleAsync(Guid snapshotId, CancellationToken ct)
    {
        await _uow.BeginTransactionAsync(ct);

        var snapshot = await _repository.GetWithItemsByIdAsync(snapshotId, ct);
        
        if (snapshot is null || snapshot.Status != OrderStatus.Created)
        {
            await _uow.RollbackTransactionAsync(ct);
            return;
        }

        snapshot.LockId = null;
        
        var isAvailable = await _presenceService.IsConnectedAsync(snapshot.RestaurantId, ct);
        
        if (!isAvailable)
        {
            if (snapshot.RetryCount < MaxRetryCount)
            {
                snapshot.RetryCount++;
                snapshot.UpdatedAt = DateTime.UtcNow;
                
                var delay = Math.Pow(2, snapshot.RetryCount - 1);
                snapshot.NextAttemptAt = DateTime.UtcNow.AddMinutes(delay);

                await _uow.SaveChangesAsync(ct);
                await _uow.CommitTransactionAsync(ct);
                return;
            }   
            
            snapshot.Status = OrderStatus.Rejected;
            
            var @event = new OrderRejectedEvent
            {
                OrderId = snapshot.OrderId,
                Reason = "Restaurant is offline"
            };

            await _outboxService.AddAsync(@event, ct);

            
            await _uow.SaveChangesAsync(ct);
            await _uow.CommitTransactionAsync(ct);
            return;
        }

        var dto = new OrderDto
        {
            Id = snapshot.Id,
            OrderDate = snapshot.OrderDate,
            RestaurantId = snapshot.RestaurantId,
            OrderItems = snapshot.OrderItems.Select(x => new OrderItemDto
            {
                Id = x.FoodItemId,
                Price = x.Price,
                Quantity = x.Quantity
            }).ToList()
        };
        
        snapshot.Status = OrderStatus.Pending;
        snapshot.UpdatedAt = DateTime.UtcNow;
        
        await _dispatcher.DispatchAsync(snapshot.RestaurantId, dto, ct);
        
        await _uow.SaveChangesAsync(ct);
        await _uow.CommitTransactionAsync(ct);
    }
}