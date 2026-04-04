using MUEats.Core.Domain.Events.Order;

namespace MUEats.Infrastructure.Adapters.Services;

public class EventRegistry
{
    private readonly Dictionary<string, Type> _types = new();

    public EventRegistry()
    {
        _types.Add("OrderPreparedEvent", typeof(OrderPreparedEvent));
        _types.Add("OrderCreatedEvent", typeof(OrderCreatedEvent));
        _types.Add("OrderAcceptedEvent", typeof(OrderAcceptedEvent));
        _types.Add("OrderPreparingEvent", typeof(OrderPreparingEvent));
    }

    public Type? GetType(string eventType)
    {
        return _types.GetValueOrDefault(eventType);
    }
}