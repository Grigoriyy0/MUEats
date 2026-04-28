using MUEats.Application.Dto.Order;
using MUEats.Core.Domain.Order;
using MUEats.Core.Domain.Order.ValueObjects;

namespace MUEats.Application.Ports;

public interface IOrdersRepository
{
    Task AddAsync(Order order, CancellationToken ct);
    
    Task<Order?> GetByIdAsync(Guid id, CancellationToken ct);
    
    Task DeleteAsync(Order order, CancellationToken ct);
    
    Task UpdateAsync(Order order, CancellationToken ct);

    Task<OrderStatus> GetStatusAsync(Guid id, CancellationToken ct);

    Task<OrderDto?> GetDtoByIdAsync(Guid orderId, CancellationToken ct);

    Task<List<OrderDto>> GetByRangeAsync(Guid userId, DateTime startDate, DateTime endDate, CancellationToken ct);
}