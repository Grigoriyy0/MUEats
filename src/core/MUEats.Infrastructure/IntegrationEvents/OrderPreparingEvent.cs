
namespace MUEats.Infrastructure.IntegrationEvents;

public class OrderPreparingEvent : IntegrationEvent
{
    public Guid OrderId { get; set; }
}