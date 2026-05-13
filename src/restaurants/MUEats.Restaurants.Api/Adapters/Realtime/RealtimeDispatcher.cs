using Microsoft.AspNetCore.SignalR;
using MUEats.Restaurants.Infrastructure.ExternalServices.Api;

namespace MUEats.Restaurants.Api.Adapters.Realtime;

public class RealtimeDispatcher
{
    private readonly IHubContext<OrdersHub> _context;

    public RealtimeDispatcher(IHubContext<OrdersHub> context)
    {
        _context = context;
    }

    public Task DispatchAsync(Guid restaurantId, OrderSnapshot snapshot, CancellationToken ct)
    {
        return _context.Clients.Groups(OrdersHub.BuildRestaurantGroup(restaurantId))
            .SendAsync("order_created", snapshot, ct);
    }
}