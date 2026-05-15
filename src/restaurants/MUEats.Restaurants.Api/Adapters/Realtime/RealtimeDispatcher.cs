using Microsoft.AspNetCore.SignalR;
using MUEats.Restaurants.Application.DTOs;
using MUEats.Restaurants.Application.Ports;

namespace MUEats.Restaurants.Api.Adapters.Realtime;

public class RealtimeDispatcher : IRealtimeDispatcher
{
    private readonly IHubContext<OrdersHub> _context;

    public RealtimeDispatcher(IHubContext<OrdersHub> context)
    {
        _context = context;
    }

    public Task DispatchAsync(Guid restaurantId, OrderDto dto, CancellationToken ct)
    {
        return _context.Clients.Groups(OrdersHub.BuildRestaurantGroup(restaurantId))
            .SendAsync("order_created", dto, ct);
    }
}