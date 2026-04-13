namespace MUEats.Core.Domain.Events.Order;

public class OrderSentEvent : DomainEvent
{
    public Guid OrderId { get; set; }
    
    public Guid RestaurantId { get; set; }
}