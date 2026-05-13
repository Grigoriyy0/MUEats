namespace MUEats.Restaurants.Infrastructure.ExternalServices.IntegrationEvents;

public class OrderCreatedEvent : IntegrationEvent
{
    public Guid OrderId { get; set; }
}