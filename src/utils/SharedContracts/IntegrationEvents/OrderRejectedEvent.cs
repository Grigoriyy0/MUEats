namespace SharedContracts.IntegrationEvents;

public class OrderRejectedEvent : IntegrationEvent
{
    public Guid OrderId { get; init; }
    
    public string Reason { get; init; }
}