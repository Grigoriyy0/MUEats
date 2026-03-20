namespace MUEats.Core.Domain.Events.Courier;

public class CourierRequestedEvent : DomainEvent
{
    public Guid OrderId { get; set; }
    
    public string DeliveryAddress { get; set; }
    
    public string RestaurantAddress { get; set; }
}