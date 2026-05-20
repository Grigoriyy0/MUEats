using MUEats.Application.IntegrationEvents;
using MUEats.Core;

namespace MUEats.Application.Ports;

public interface IInboxService
{
    Task AddAsync(IntegrationEvent message, CancellationToken ct);
    Task ProcessMessageAsync(InboxMessage message, CancellationToken ct);
}