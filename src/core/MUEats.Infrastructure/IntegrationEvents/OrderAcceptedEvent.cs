using MUEats.Core.Domain.Events;

namespace MUEats.Infrastructure.IntegrationEvents;

public class OrderAcceptedEvent : IntegrationEvent
{
    public Guid OrderId { get; set; }
}