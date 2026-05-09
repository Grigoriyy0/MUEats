using MUEats.Application.IntegrationEvents;

namespace MUEats.Infrastructure.Adapters.Services;

public class TopicMapper
{
    private readonly Dictionary<Type, string> _topics = new();

    public TopicMapper()
    {
        _topics[typeof(OrderCreatedEvent)] = "restaurants";
        _topics[typeof(OrderAcceptedEvent)] = "orders";
        _topics[typeof(OrderPreparedEvent)] = "orders";
        _topics[typeof(OrderPickedUpEvent)] = "orders";
    }

    public string? TryGetTopic(Type type)
    {
        return _topics.GetValueOrDefault(type);
    }
}