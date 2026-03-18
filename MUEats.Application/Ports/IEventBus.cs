using MUEats.Core.Domain.Events;

namespace MUEats.Application.Ports;

public interface IEventBus
{
    Task PublishAsync<T>(T @event, CancellationToken ct) where T : DomainEvent;
    
    void Subscribe<T>(Func<T, Task> handler) where T : DomainEvent;
}