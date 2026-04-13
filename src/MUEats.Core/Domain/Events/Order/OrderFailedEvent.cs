namespace MUEats.Core.Domain.Events.Order;

public class OrderFailedEvent : DomainEvent
{
    public Guid OrderId { get; set; }
    
    public string Message { get; set; }
}