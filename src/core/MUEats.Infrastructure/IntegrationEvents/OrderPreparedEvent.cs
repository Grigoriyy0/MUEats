
namespace MUEats.Infrastructure.IntegrationEvents;

public class OrderPreparedEvent : IntegrationEvent
{
    public Guid OrderId { get; set; }
}