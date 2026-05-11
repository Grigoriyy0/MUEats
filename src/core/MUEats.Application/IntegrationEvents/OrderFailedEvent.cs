
namespace MUEats.Application.IntegrationEvents;

public class OrderFailedEvent : IntegrationEvent
{
    public Guid OrderId { get; set; }
    
    public string FailureReason { get; set; }
}