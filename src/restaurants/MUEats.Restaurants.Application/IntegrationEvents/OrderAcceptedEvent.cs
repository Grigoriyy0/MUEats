using SharedContracts.IntegrationEvents;

namespace MUEats.Restaurants.Application.IntegrationEvents;

public class OrderAcceptedEvent : IntegrationEvent
{
    public Guid OrderId { get; init; }
}