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

    public async Task DispatchAsync(IntegrationEvent @event, CancellationToken ct)
    {
        var eventType = @event.GetType();
        var handlerType = typeof(IIntegrationEventHandler<>).MakeGenericType(eventType);
    
        var handler = _serviceProvider.GetRequiredService(handlerType);

        var method = handlerType.GetMethod(nameof(IIntegrationEventHandler<>.HandleAsync));
    
        if (method != null)
        {
            await (Task)method.Invoke(handler, [@event, ct])!;
        }
    }
}