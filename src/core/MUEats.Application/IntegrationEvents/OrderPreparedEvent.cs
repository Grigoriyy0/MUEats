
namespace MUEats.Application.IntegrationEvents;

public class OrderPreparedEvent : IntegrationEvent
{
    public Guid OrderId { get; set; }
}