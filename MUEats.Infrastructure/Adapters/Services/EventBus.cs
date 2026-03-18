using MUEats.Application.Ports;
using MUEats.Core.Domain.Events;

namespace MUEats.Infrastructure.Adapters.Services;
using System.Threading.Channels;

public class InMemoryEventBus : IEventBus
{
    private readonly Channel<DomainEvent> _channel = Channel.CreateUnbounded<DomainEvent>();
    private readonly Dictionary<Type, List<Func<DomainEvent, Task>>> _handlers = new();

    public InMemoryEventBus()
    {
        StartWorker();
    }

    public async Task PublishAsync<T>(T @event, CancellationToken ct) where T : DomainEvent
    {
        await _channel.Writer.WriteAsync(@event, ct);
    }

    public void Subscribe<T>(Func<T, Task> handler) where T : DomainEvent
    {
        var type = typeof(T);

        if (!_handlers.ContainsKey(type))
            _handlers[type] = new List<Func<DomainEvent, Task>>();

        _handlers[type].Add(e => handler((T)e));
    }

    private void StartWorker()
    {
        Task.Run(async () =>
        {
            await foreach (var message in _channel.Reader.ReadAllAsync())
            {
                var type = message.GetType();

                if (_handlers.TryGetValue(type, out var handlers))
                {
                    foreach (var handler in handlers)
                    {
                        _ = Task.Run(async () =>
                        {
                            try
                            {
                                await handler(message);
                            }
                            catch (Exception ex)
                            {
                                Console.WriteLine($"Handler error: {ex}");
                            }
                        });
                    }
                }
            }
        });
    }
}