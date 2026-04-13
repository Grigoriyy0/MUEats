using Microsoft.Extensions.DependencyInjection;
using MUEats.Application.Helpers;
using MUEats.Application.Ports;
using MUEats.Infrastructure.Handlers;
using Newtonsoft.Json;

namespace MUEats.Infrastructure.Adapters.Services;

public class EventDispatcher : IEventDispatcher
{
    private readonly EventsRegistry _eventsRegistry;
    private readonly IServiceScopeFactory _serviceScopeFactory;

    public EventDispatcher(EventsRegistry eventsRegistry, IServiceScopeFactory serviceScopeFactory)
    {
        _eventsRegistry = eventsRegistry;
        _serviceScopeFactory = serviceScopeFactory;
    }

    public async Task DispatchAsync(string eventType, string message, CancellationToken ct)
    {
        var type = _eventsRegistry.GetType(eventType);
        
        if (type == null)
        {
            return;
        }
        
        await using var scope = _serviceScopeFactory.CreateAsyncScope();
        
        var @event = JsonConvert.DeserializeObject(message, type, JsonSerializerHelper.Settings);
        
        var handlerType = typeof(IIntegrationEventHandler<>).MakeGenericType(type);
        
        var handler = scope.ServiceProvider.GetService(handlerType);

        if (handler != null)
        {
            var method = handlerType.GetMethod("HandleAsync");
            
            await (Task)method!.Invoke(handler, [@event, ct])!;
        }
    }
}