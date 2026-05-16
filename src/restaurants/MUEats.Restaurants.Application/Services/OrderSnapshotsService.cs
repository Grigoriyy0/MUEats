using CSharpFunctionalExtensions;
using MUEats.Restaurants.Application.DTOs;
using MUEats.Restaurants.Application.IntegrationEvents;
using MUEats.Restaurants.Application.Ports;
using MUEats.Restaurants.Core.Projections.Order;
using Primitives;

namespace MUEats.Restaurants.Application.Services;

public class OrderSnapshotsService
{
    private readonly IOrderSnapshotsRepository _repository;
    private readonly IUnitOfWork _uow;
    private readonly IOutboxService _outboxService;
    
    public OrderSnapshotsService(IOrderSnapshotsRepository repository, 
        IUnitOfWork uow, 
        IOutboxService outboxService)
    {
        _repository = repository;
        _uow = uow;
        _outboxService = outboxService;
    }

    public Task<List<OrderDto>> GetPendingAsync(Guid restaurantId, CancellationToken ct)
    {
        return _repository.GetPendingAsync(restaurantId, ct);
    }

    public async Task<UnitResult<Error>> AcceptAsync(Guid orderId, Guid restaurantId, CancellationToken ct)
    {
        await _uow.BeginTransactionAsync(ct);
        
        var orderSnapshot = await _repository.GetByIdAsync(orderId, ct);

        if (orderSnapshot is null)
        {
            await _uow.RollbackTransactionAsync(ct);
            return ApplicationErrors.OrderSnapshot.NotFound;
        }

        if (orderSnapshot.RestaurantId != restaurantId)
        {
            await _uow.RollbackTransactionAsync(ct);
            return ApplicationErrors.OrderSnapshot.WrongRestaurant;
        }

        orderSnapshot.Status = OrderStatus.Accepted;
        orderSnapshot.UpdatedAt = DateTime.UtcNow;

        var @event = new OrderAcceptedEvent
        {
            OrderId = orderSnapshot.OrderId
        };

        await _outboxService.AddAsync(@event, ct);

        await _uow.SaveChangesAsync(ct);
        await _uow.CommitTransactionAsync(ct);
        
        return UnitResult.Success<Error>();
    }

    public async Task<UnitResult<Error>> RejectAsync(Guid orderId, Guid restaurantId, string reason, CancellationToken ct)
    {
        await _uow.BeginTransactionAsync(ct);
        
        var orderSnapshot = await _repository.GetByIdAsync(orderId, ct);

        if (orderSnapshot is null)
        {
            await _uow.RollbackTransactionAsync(ct);
            return ApplicationErrors.OrderSnapshot.NotFound;
        }

        if (orderSnapshot.RestaurantId != restaurantId)
        {
            await _uow.RollbackTransactionAsync(ct);
            return ApplicationErrors.OrderSnapshot.WrongRestaurant;
        }

        orderSnapshot.Status = OrderStatus.Rejected;
        orderSnapshot.UpdatedAt = DateTime.UtcNow;

        var @event = new OrderRejectedEvent
        {
            OrderId = orderSnapshot.OrderId,
            Reason = reason
        };

        await _outboxService.AddAsync(@event, ct);

        await _uow.SaveChangesAsync(ct);
        await _uow.CommitTransactionAsync(ct);
        
        return UnitResult.Success<Error>();
    }
}