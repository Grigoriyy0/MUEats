using Microsoft.EntityFrameworkCore;
using MUEats.Application.Ports;
using MUEats.Core.Domain.Order.Entities;
using MUEats.Infrastructure.Persistence;

namespace MUEats.Infrastructure.Adapters.Repositories;

public class OrderSagasRepository : IOrderSagasRepository
{
    private readonly MueDbContext _dbContext;

    public OrderSagasRepository(MueDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public Task AddAsync(OrderSaga state, CancellationToken ct)
    {
        return _dbContext.OrderSagas.AddAsync(state, ct)
            .AsTask();
    }

    public Task<OrderSaga?> GetByIdAsync(Guid correlationId, CancellationToken ct)
    {
        return _dbContext.Database.SqlQueryRaw<OrderSaga?>("""
                                                    SELECT * FROM "OrderSagas"
                                                    WHERE "CorrelationId" = {0}
                                                    FOR UPDATE
                                                    """, correlationId)
            .FirstOrDefaultAsync(ct);
    }
}