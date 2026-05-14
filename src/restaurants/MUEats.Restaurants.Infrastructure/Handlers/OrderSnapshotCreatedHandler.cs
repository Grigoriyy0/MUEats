using MUEats.Restaurants.Application.Ports;
using MUEats.Restaurants.Infrastructure.ExternalServices.Api;
using MUEats.Restaurants.Infrastructure.Handlers.Interfaces;
using MUEats.Restaurants.Infrastructure.Persistence.Contexts;

namespace MUEats.Restaurants.Infrastructure.Handlers;

public class OrderSnapshotCreatedHandler : IOrderSnapshotCreatedHandler
{
    private readonly RestaurantsDbContext _context;
    private readonly IRealtimeDispatcher _dispatcher;

    public OrderSnapshotCreatedHandler(RestaurantsDbContext context, IRealtimeDispatcher dispatcher)
    {
        _context = context;
        _dispatcher = dispatcher;
    }

    public async Task HandleAsync(OrderSnapshot snapshot, CancellationToken ct)
    {
        
    }
}