using MUEats.Core.Domain.Events.Order;

namespace MUEats.Infrastructure.Adapters.Services;

public class TopicMapper
{
    private readonly Dictionary<Type, string> _topics = new();

    public TopicMapper()
    {
        _topics[typeof(OrderCreatedEvent)] = "order.created";
    }
    
    public string? TryGetTopic(Type type)
    {
        return _topics.GetValueOrDefault(type);
    }
}