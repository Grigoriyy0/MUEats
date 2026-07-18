namespace SharedContracts.IntegrationEvents;

public class RestaurantDeletedEvent : IntegrationEvent
{
    public Guid RestaurantId { get; init; }
}