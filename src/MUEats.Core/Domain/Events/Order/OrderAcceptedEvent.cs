namespace MUEats.Core.Domain.Events.Order;

public class OrderAcceptedEvent : DomainEvent
{
    public Guid OrderId { get; set; }
    
    public string RestaurantAddress { get; set; }
    
    public string ToAddress { get; set; }
    
    public decimal DeliveryReward { get; set; }
}