namespace MUEats.Core.Domain.Events.Order;

public class OrderSentEvent
{
    public Guid OrderId { get; set; }
    
    public Guid RestaurantId { get; set; }
}