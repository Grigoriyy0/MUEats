namespace MUEats.Restaurants.Application.IntegrationEvents;

public class OrderRejectedEvent : IntegrationEvent
{
    public Guid OrderId { get; set; }
    
    public string Reason { get; set; }
}