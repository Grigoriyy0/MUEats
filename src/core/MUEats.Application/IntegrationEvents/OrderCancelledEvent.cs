namespace MUEats.Application.IntegrationEvents;

public class OrderCancelledEvent : IntegrationEvent
{
    public Guid OrderId { get; set; }
}