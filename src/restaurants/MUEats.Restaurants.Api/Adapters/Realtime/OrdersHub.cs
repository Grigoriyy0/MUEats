using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using MUEats.Restaurants.Application.Ports;

namespace MUEats.Restaurants.Api.Adapters.Realtime;

[Authorize]
public sealed class OrdersHub : Hub
{
    private readonly IPresenceService _presenceService;

    public OrdersHub(IPresenceService presenceService)
    {
        _presenceService = presenceService;
    }

    public override async Task OnConnectedAsync()
    {
        var restaurantId = GetRestaurantId();
        var group = BuildRestaurantGroup(restaurantId);
        
        await Groups.AddToGroupAsync(Context.ConnectionId, group);
        await _presenceService.RegisterConnectionAsync(restaurantId, Context.ConnectionId);
        
        await base.OnConnectedAsync();
    }

    public override async Task OnDisconnectedAsync(Exception? exception)
    {
        var restaurantId = GetRestaurantId();
        var group = BuildRestaurantGroup(restaurantId);
        
        await Groups.RemoveFromGroupAsync(Context.ConnectionId, group);
        await _presenceService.UnregisterConnectionAsync(restaurantId, Context.ConnectionId);
        
        await base.OnDisconnectedAsync(exception);
    }

    private Guid GetRestaurantId()
    {
        var claim = Context.User?.FindFirst("restaurant_id")?.Value;

        if (!Guid.TryParse(claim, out var restaurantId))
        {
            throw new HubException("Restaurant id claim missing.");
        }

        return restaurantId;
    }

    public static string BuildRestaurantGroup(Guid restaurantId)
    {
        return $"restaurant:{restaurantId}";
    }
}