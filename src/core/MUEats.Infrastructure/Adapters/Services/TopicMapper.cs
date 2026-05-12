using MUEats.Application.IntegrationEvents;

namespace MUEats.Infrastructure.Adapters.Services;

public class TopicMapper
{
    private readonly Dictionary<Type, string> _topics = new();

    public TopicMapper()
    {
        _topics[typeof(OrderCreatedEvent)] = "orders.created";
        _topics[typeof(OrderAcceptedEvent)] = "orders.accepted";
        _topics[typeof(OrderPreparedEvent)] = "orders.prepared";
    }

    public string? TryGetTopic(Type type)
    {
        return _topics.GetValueOrDefault(type);
    }
}