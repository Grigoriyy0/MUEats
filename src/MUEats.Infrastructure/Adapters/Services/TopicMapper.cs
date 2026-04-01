using MUEats.Core.Domain.Events.Order;

namespace MUEats.Infrastructure.Adapters.Services;

public class TopicMapper
{
    private readonly Dictionary<Type, string> _topics = new Dictionary<Type, string>();

    public TopicMapper()
    {
        _topics[typeof(OrderCreatedEvent)] = "orders";
        _topics[typeof(OrderAcceptedEvent)] = "orders";
        _topics[typeof(OrderFailedEvent)] = "orders";
        _topics[typeof(OrderPreparingEvent)] = "orders";
        _topics[typeof(OrderPreparedEvent)] = "orders";
    }

    public string? TryGetTopic(Type type)
    {
        return _topics.GetValueOrDefault(type);
    }
}