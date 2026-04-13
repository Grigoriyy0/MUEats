using MUEats.Application.Ports;
using MUEats.Core;
using MUEats.Infrastructure.Persistence;

namespace MUEats.Infrastructure.Adapters.Repositories;

public class OutboxRepository(MueDbContext context) : IOutboxRepository
{
    public Task AddAsync(OutboxMessage message, CancellationToken ct)
    {
        return context.OutboxMessages.AddAsync(message, ct)
            .AsTask();
    }
}