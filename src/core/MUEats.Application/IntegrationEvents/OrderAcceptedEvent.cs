
namespace MUEats.Application.IntegrationEvents;

public class OrderAcceptedEvent : IntegrationEvent
{
    public Guid OrderId { get; set; }
}