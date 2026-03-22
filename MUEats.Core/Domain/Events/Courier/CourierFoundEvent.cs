namespace MUEats.Core.Domain.Events.Courier;

public class CourierFoundEvent : DomainEvent
{
    public Guid OrderId { get; set; }
    
    public Guid CourierId { get; set; }
    
    public DateTime EstimatedArrival { get; set; }
}