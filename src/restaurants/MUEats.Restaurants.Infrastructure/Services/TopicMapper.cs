using MUEats.Restaurants.Infrastructure.ExternalServices.IntegrationEvents;

namespace MUEats.Restaurants.Infrastructure.Services;

public class TopicMapper
{
    private readonly Dictionary<Type, string> _topics = new();

    public TopicMapper()
    {
        _topics[typeof(OrderCreatedEvent)] = "orders.created";
    }

    public string? TryGetTopic(Type type)
    {
        return _topics.GetValueOrDefault(type);
    }
}