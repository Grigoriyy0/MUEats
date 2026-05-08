using Microsoft.EntityFrameworkCore;
using MUEats.Application.Dto.Order;
using MUEats.Application.Ports;
using MUEats.Core.Domain.Order.ValueObjects;
using MUEats.Infrastructure.Persistence;

namespace MUEats.Infrastructure.Adapters.Services;

public class OrdersQueries : IOrdersQueries
{
    private readonly MueDbContext _context;

    public OrdersQueries(MueDbContext context)
    {
        _context = context;
    }

    public Task<OrderStatus> GetStatusAsync(Guid id, CancellationToken ct)
    {
        return _context.Orders.Where(x => x.Id == id)
            .Select(y => y.Status)
            .FirstOrDefaultAsync(ct);
    }

    public Task<OrderDto?> GetDtoByIdAsync(Guid orderId, CancellationToken ct)
    {
        return _context.Orders.Where(x => x.Id == orderId)
            .Include(y => y.OrderItems)
            .Select(x => new OrderDto
            {
                Id = x.Id,
                RestaurantId = x.RestaurantId,
                OrderItems = x.OrderItems.Select(o => new OrderItemDto
                {
                    Id = o.Id,
                    ItemName = o.Name,
                    Price = o.Price
                }).ToList()
            }).FirstOrDefaultAsync(ct);
    }
}