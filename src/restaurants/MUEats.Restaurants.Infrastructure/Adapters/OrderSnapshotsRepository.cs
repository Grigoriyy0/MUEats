using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using MUEats.Restaurants.Application.DTOs;
using MUEats.Restaurants.Application.Ports;
using MUEats.Restaurants.Core.Projections.Order;
using MUEats.Restaurants.Infrastructure.Persistence.Contexts;

namespace MUEats.Restaurants.Infrastructure.Adapters;

public class OrderSnapshotsRepository : IOrderSnapshotsRepository
{
    private readonly RestaurantsDbContext _context;
    
    public OrderSnapshotsRepository(RestaurantsDbContext context)
    {
        _context = context;
    }

    public Task<List<OrderDto>> GetPendingAsync(Guid restaurantId, CancellationToken ct)
    {
        return _context.OrderSnapshots.Where(x => x.RestaurantId == restaurantId && x.Status == OrderStatus.Pending)
            .Include(o => o.OrderItems)
            .Select(y => new OrderDto
            {
                Id = y.Id,
                OrderDate = y.OrderDate,
                RestaurantId = restaurantId,
                OrderItems = y.OrderItems.Select(z => new OrderItemDto
                {
                    Id = z.FoodItemId,
                    ItemName = z.ItemName,
                    Price = z.Price,
                    Quantity = z.Quantity
                }).ToList()
            })
            .ToListAsync(ct);
    }

    public Task<OrderSnapshot?> GetByOrderIdAsync(Guid orderId, CancellationToken ct)
    {
        return _context.OrderSnapshots
            .FirstOrDefaultAsync(x => x.OrderId == orderId, ct);
    }

    public Task<OrderSnapshot?> GetByIdAsync(Guid snapshotId, CancellationToken ct)
    {
        return _context.OrderSnapshots
            .FirstOrDefaultAsync(x => x.Id == snapshotId, ct);
    }

    public Task<OrderSnapshot?> GetWithItemsByIdAsync(Guid snapshotId, CancellationToken ct)
    {
        return _context.OrderSnapshots
            .Include(y => y.OrderItems)
            .FirstOrDefaultAsync(x => x.Id == snapshotId, ct);
    }
}