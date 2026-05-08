using Microsoft.EntityFrameworkCore;
using MUEats.Application.Ports;
using MUEats.Core.Domain.Order.Entities;
using MUEats.Infrastructure.Persistence;

namespace MUEats.Infrastructure.Adapters.Repositories;

public class OrderSagaStatesRepository(MueDbContext dbContext) : IOrderSagaStatesRepository
{
    public Task AddAsync(OrderSagaState state, CancellationToken ct)
    {
        return dbContext.OrderSagaStates.AddAsync(state, ct)
            .AsTask();
    }

    public Task<OrderSagaState?> GetByIdAsync(Guid correlationId, CancellationToken ct)
    {
        return dbContext.OrderSagaStates
            .FirstOrDefaultAsync(x => x.CorrelationId == correlationId, ct);
    }

    public Task UpdateAsync(OrderSagaState state, CancellationToken ct)
    {
        dbContext.OrderSagaStates.Update(state);
        return Task.CompletedTask;
    }
}