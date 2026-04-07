using MUEats.Core.Domain.Events.Order;

namespace MUEats.Infrastructure.Adapters.Services;

public class TopicMapper
{
    private readonly Dictionary<Type, string> _topics = new();

    public TopicMapper()
    {
        _topics[typeof(OrderCreatedEvent)] = "orders";
        _topics[typeof(OrderAcceptedEvent)] = "restaurants";
        _topics[typeof(OrderPreparingEvent)] = "restaurants";
        _topics[typeof(OrderPreparedEvent)] = "restaurants";
        _topics[typeof(OrderPickedUpEvent)] = "restaurants";
    }

    public string? TryGetTopic(Type type)
    {
        return _topics.GetValueOrDefault(type);
    }
}