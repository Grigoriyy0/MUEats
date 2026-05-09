
namespace MUEats.Application.IntegrationEvents;

public class OrderCreatedEvent : IntegrationEvent
{
    public Guid OrderId { get; set; }
}