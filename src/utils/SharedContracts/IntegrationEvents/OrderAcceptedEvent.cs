namespace SharedContracts.IntegrationEvents;

public class OrderAcceptedEvent : IntegrationEvent
{
    public Guid OrderId { get; init; }

    public Guid UserId { get; init; }
    
    public string ContactInfo { get; init; }
    
    public DateTime PickUpTime { get; set; }
    
    public decimal TotalAmount { get; set; }

}