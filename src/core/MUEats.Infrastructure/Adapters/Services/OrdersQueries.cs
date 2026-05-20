using Microsoft.EntityFrameworkCore;
using MUEats.Application.Dto.Order;
using MUEats.Application.Ports;
using MUEats.Application.Queries;
using MUEats.Core.Domain.Order.ValueObjects;
using MUEats.Infrastructure.Persistence;

namespace MUEats.Infrastructure.Adapters.Services;

public class OrdersQueries : IOrdersQueries
{
    private readonly MueDbContext _context;
    private readonly ICurrentUserContext _currentUserContext;
    
    public OrdersQueries(MueDbContext context, ICurrentUserContext currentUserContext)
    {
        _context = context;
        _currentUserContext = currentUserContext;
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

    public Task<List<OrderDto>> GetHistoryAsync(GetOrdersHistoryQuery query, CancellationToken ct)
    {
        var userId = _currentUserContext.GetUserId();
        
        return _context.Orders.Where(x => x.CreatedAt >= query.FromDate && x.CreatedAt <= query.ToDate && x.UserId == userId)
            .Select(y => new OrderDto
            {
                Id = y.Id,
                OrderDate = y.CreatedAt,
                RestaurantDetails = y.RestaurantId.ToString(),
                OrderItems = y.OrderItems.Select(z => new OrderItemDto
                {
                    Id = z.Id,
                    ItemName = z.Name,
                    Price = z.Price
                }).ToList(),
                TotalPrice = y.TotalPrice
            })
            .OrderBy(z => z.OrderDate)
            .Skip((query.Page - 1) * query.PageSize)
            .Take(query.PageSize)
            .ToListAsync(ct);
    }
}