using System.Collections.Concurrent;
using Microsoft.Extensions.Hosting;
using MUEats.Application.Ports;
using MUEats.Core.Domain.Events;

namespace MUEats.Infrastructure.Adapters.Services;
using System.Threading.Channels;

public class InMemoryEventBus : BackgroundService, IEventBus
{
    private readonly Channel<DomainEvent> _channel = Channel.CreateUnbounded<DomainEvent>();
    private readonly ConcurrentDictionary<Type, ConcurrentBag<Func<DomainEvent, Task>>> _handlers = new();
    private readonly CancellationTokenSource _cts = new();
    
    public async Task PublishAsync<T>(T @event, CancellationToken ct) where T : DomainEvent
    {
        await _channel.Writer.WriteAsync(@event, ct);
    }

    public void Subscribe<T>(Func<T, Task> handler) where T : DomainEvent
    {
        var type = typeof(T);

        _handlers.GetOrAdd(type, _ => new ConcurrentBag<Func<DomainEvent, Task>>())
            .Add(e => handler((T)e));
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        using var cts = CancellationTokenSource.CreateLinkedTokenSource(stoppingToken, _cts.Token);
        
        await foreach (var message in _channel.Reader.ReadAllAsync(cts.Token))
        {
            try
            {
                var type = message.GetType();

                if (_handlers.TryGetValue(type, out var handlers))
                {
                    var tasks = handlers.Select(handler => handler(message));
                    await Task.WhenAll(tasks);
                }
            }
            catch (Exception ex)
            {
                // _logger.LogError(ex, "Error processing event {EventType}", message.GetType().Name);
            }
        }
    }

    public override async Task StopAsync(CancellationToken cancellationToken)
    {
        await _cts.CancelAsync();
        _channel.Writer.TryComplete(); 
        await base.StopAsync(cancellationToken);
    }

    public override void Dispose()
    {
        _cts.Dispose();
        base.Dispose();
    }
}