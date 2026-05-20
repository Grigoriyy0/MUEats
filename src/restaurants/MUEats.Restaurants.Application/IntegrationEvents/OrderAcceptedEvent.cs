namespace MUEats.Restaurants.Application.IntegrationEvents;

public class OrderAcceptedEvent : IntegrationEvent
{
    public Guid OrderId { get; set; }
}