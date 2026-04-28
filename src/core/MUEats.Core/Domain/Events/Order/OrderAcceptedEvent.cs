namespace MUEats.Core.Domain.Events.Order;

public class OrderAcceptedEvent : DomainEvent
{
    public Guid OrderId { get; set; }
}