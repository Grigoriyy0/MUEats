
namespace MUEats.Infrastructure.IntegrationEvents;

public class OrderCreatedEvent : IntegrationEvent
{
    public Guid OrderId { get; set; }
}