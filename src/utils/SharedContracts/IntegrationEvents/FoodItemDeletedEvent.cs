namespace SharedContracts.IntegrationEvents;

public class FoodItemDeletedEvent : IntegrationEvent
{
    public Guid RestaurantId { get; init; }
    
    public Guid FoodItemId { get; init; }
}