namespace MUEats.Core.Domain.Events.Order;

public class OrderCreatedEvent : DomainEvent
{
    public Guid OrderId { get; set; }
}