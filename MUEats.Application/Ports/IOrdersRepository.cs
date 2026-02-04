using MUEats.Core.Domain.Order;

namespace MUEats.Application.Ports;

public interface IOrdersRepository
{
    Task AddAsync(Order order, CancellationToken ct);
    
    Task<Order?> GetByIdAsync(Guid id, CancellationToken ct);
    
    Task DeleteAsync(Order order, CancellationToken ct);
    
    Task UpdateAsync(Order order, CancellationToken ct);
}