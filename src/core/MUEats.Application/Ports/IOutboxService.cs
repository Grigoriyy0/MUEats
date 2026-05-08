using MUEats.Core.Domain.Events;

namespace MUEats.Application.Ports;

public interface IOutboxService
{
    Task CreateAsync(DomainEvent @event, CancellationToken ct);
}