using MUEats.Core.Domain.Order.Entities;

namespace MUEats.Application.Ports;

public interface IOrderSagaStatesRepository
{
    Task AddAsync(OrderSagaState state, CancellationToken ct);
    
    Task<OrderSagaState?> GetByIdAsync(Guid correlationId, CancellationToken ct);
    
    Task UpdateAsync(OrderSagaState state, CancellationToken ct);
}