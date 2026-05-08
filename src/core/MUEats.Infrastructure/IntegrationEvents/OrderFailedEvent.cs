
namespace MUEats.Infrastructure.IntegrationEvents;

public class OrderFailedEvent : IntegrationEvent
{
    public Guid OrderId { get; set; }
    
    public string Message { get; set; }
}