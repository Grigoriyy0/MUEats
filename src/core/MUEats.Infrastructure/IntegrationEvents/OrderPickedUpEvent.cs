namespace MUEats.Infrastructure.IntegrationEvents;

public class OrderPickedUpEvent : IntegrationEvent
{
    public Guid OrderId { get; set; }
}