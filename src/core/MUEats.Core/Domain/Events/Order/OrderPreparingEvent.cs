namespace MUEats.Core.Domain.Events.Order;

public class OrderPreparingEvent : DomainEvent
{
    public Guid OrderId { get; set; }
}