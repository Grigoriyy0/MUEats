using Microsoft.EntityFrameworkCore;
using MUEats.Application.Dto.Order;
using MUEats.Application.Ports;
using MUEats.Core.Domain.Order;
using MUEats.Core.Domain.Order.ValueObjects;
using MUEats.Infrastructure.Persistence;

namespace MUEats.Infrastructure.Adapters.Repositories;

public class OrdersRepository(MueDbContext context) : IOrdersRepository
{
    public Task AddAsync(Order order, CancellationToken ct)
    {
        return context.Orders.AddAsync(order, ct)
            .AsTask();
    }

    public Task<Order?> GetByIdAsync(Guid id, CancellationToken ct)
    {
        return context.Orders.Where(x => x.Id == id)
            .Include(x => x.OrderItems)
            .FirstOrDefaultAsync(ct);
    }

    public Task DeleteAsync(Order order, CancellationToken ct)
    {
        context.Orders.Remove(order);
        
        return Task.CompletedTask;
    }

    public Task UpdateAsync(Order order, CancellationToken ct)
    {
        context.Orders.Update(order);
        
        return Task.CompletedTask;
    }

    public Task<OrderStatus?> GetStatusAsync(Guid id, CancellationToken ct)
    {
        return context.Orders
            .Where(x => x.Id == id)
            .Select(x => (OrderStatus?)x.Status)
            .FirstOrDefaultAsync(ct);
    }

    public Task<OrderDto?> GetDtoByIdAsync(Guid orderId, CancellationToken ct)
    {
        return context.Orders.Where(x => x.Id == orderId)
            .Include(y => y.OrderItems)
            .Select(x => new OrderDto
            {
                Id = x.Id,
                Address = x.Address,
                TotalPrice = x.Price,
                OrderItems = x.OrderItems.Select(o => new OrderItemDto
                {
                    Id = o.Id,
                    ItemName = o.Name,
                    Price = o.Price
                }).ToList()
            }).FirstOrDefaultAsync(ct);
    }
}