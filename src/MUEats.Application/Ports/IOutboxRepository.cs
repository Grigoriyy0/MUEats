using MUEats.Core;

namespace MUEats.Application.Ports;

public interface IOutboxRepository
{
    Task AddAsync(OutboxMessage message, CancellationToken ct);
}