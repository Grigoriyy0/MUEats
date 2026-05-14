using MUEats.Restaurants.Application.DTOs;
using MUEats.Restaurants.Application.IntegrationEvents;
using MUEats.Restaurants.Application.Ports;
using MUEats.Restaurants.Core.Projections.Order;
using MUEats.Restaurants.Infrastructure.Handlers.Interfaces;

namespace MUEats.Restaurants.Application.Handlers;

public class OrderSnapshotCreatedHandler : IOrderSnapshotCreatedHandler
{
    private readonly IRealtimeDispatcher _dispatcher;
    private readonly IPresenceService _presenceService;
    private const int MaxRetryCount = 3;
    
    public OrderSnapshotCreatedHandler(IRealtimeDispatcher dispatcher, IPresenceService presenceService)
    {
        _dispatcher = dispatcher;
        _presenceService = presenceService;
    }

    public async Task HandleAsync(OrderSnapshot snapshot, CancellationToken ct)
    {
        if (snapshot.Status != OrderStatus.Created)
        {
            return;
        }
        
        var isAvailable = await _presenceService.IsConnectedAsync(snapshot.RestaurantId, ct);
        
        if (!isAvailable)
        {
            if (snapshot.RetryCount < MaxRetryCount)
            {
                snapshot.RetryCount++;
                snapshot.UpdatedAt = DateTime.UtcNow;
                
                var delay = Math.Pow(2, snapshot.RetryCount - 1);
                snapshot.NextAttemptAt = DateTime.UtcNow.AddMinutes(delay);

                return;
            }   
            
            snapshot.Status = OrderStatus.Rejected;
            
            var @event = new OrderRejectedEvent
            {
                OrderId = snapshot.OrderId,
                Reason = "Restaurant is offline"
            };
            
            // publish

            return;
        }

        var dto = new OrderCreatedDto();

        snapshot.Status = OrderStatus.Pending;
        snapshot.UpdatedAt = DateTime.UtcNow;
        
        await _dispatcher.DispatchAsync(snapshot.RestaurantId, dto, ct);
    }
}