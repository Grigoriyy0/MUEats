namespace MUEats.Core.Domain.Events.Order;

public class OrderDeliveredEvent : DomainEvent
{
    public Guid OrderId { get; set; }
}