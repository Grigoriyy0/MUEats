namespace MUEats.Core.Domain.Events.Order;

public class OrderPreparedEvent : DomainEvent
{
    public Guid OrderId { get; set; }
}