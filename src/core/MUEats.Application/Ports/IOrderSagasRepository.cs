using MUEats.Core.Domain.Order.Entities;

namespace MUEats.Application.Ports;

public interface IOrderSagasRepository
{
    Task AddAsync(OrderSaga state, CancellationToken ct);
    
    Task<OrderSaga?> GetByIdAsync(Guid correlationId, CancellationToken ct);
}