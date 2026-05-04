namespace MUEats.Restaurants.Core.Domain.Events;

public class MenuCreatedEvent : DomainEvent
{
    public Guid RestaurantId { get; init; }
    
    public Guid MenuId { get; init; }
}