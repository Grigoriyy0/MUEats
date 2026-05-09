namespace MUEats.Application.IntegrationEvents;

public class OrderPickedUpEvent : IntegrationEvent
{
    public Guid OrderId { get; set; }
}