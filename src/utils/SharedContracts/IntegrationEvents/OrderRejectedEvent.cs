namespace SharedContracts.IntegrationEvents;

public class OrderRejectedEvent : IntegrationEvent
{
    public Guid OrderId { get; init; }
    
    public Guid UserId { get; init; }
    
    public string ContactInfo { get; init; }
    
    public string Reason { get; init; }
}