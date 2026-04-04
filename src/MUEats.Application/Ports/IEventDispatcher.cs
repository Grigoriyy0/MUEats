namespace MUEats.Application.Ports;

public interface IEventDispatcher
{
    Task DispatchAsync(string eventType, string message, CancellationToken ct);
}