using Microsoft.EntityFrameworkCore;
using MUEats.Core.Domain.Order;
using MUEats.Infrastructure.Persistence;

namespace MUEats.Infrastructure.Adapters.Repositories;

public class OrdersRepository(MueDbContext context)
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
}