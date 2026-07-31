using SharedContracts.IntegrationEvents;

namespace MUEats.Restaurants.Infrastructure.Services;

public class TopicMapper
{
    private readonly Dictionary<Type, string> _topics = new();

    public TopicMapper()
    {
        _topics[typeof(OrderCreatedEvent)] = "orders.created";
        _topics[typeof(OrderRejectedEvent)] = "orders.rejected";
        _topics[typeof(FoodItemDeletedEvent)] = "fooditems.deleted";
    }

    public string? TryGetTopic(Type type)
    {
        return _topics.GetValueOrDefault(type);
    }
}