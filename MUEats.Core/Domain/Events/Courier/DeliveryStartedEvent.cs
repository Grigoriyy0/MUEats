namespace MUEats.Core.Domain.Events.Courier;

public class DeliveryStartedEvent : DomainEvent
{
    public Guid OrderId { get; set; }
}