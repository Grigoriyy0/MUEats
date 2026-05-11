using Microsoft.Extensions.DependencyInjection;
using MUEats.Application.Handlers;
using MUEats.Application.IntegrationEvents;
using MUEats.Application.Ports;

namespace MUEats.Infrastructure.Adapters.Services;

public class EventDispatcher : IEventDispatcher
{
    private readonly IServiceProvider _serviceProvider;

    public EventDispatcher(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
    }

    public Task DispatchAsync<T>(T @event, CancellationToken ct) where T : IntegrationEvent
    {
        var handler = _serviceProvider.GetRequiredService<IIntegrationEventHandler<T>>();
        
        
        
        return handler.HandleAsync(@event, ct);
    }
}