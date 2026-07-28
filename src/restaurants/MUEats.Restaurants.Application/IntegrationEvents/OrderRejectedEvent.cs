using SharedContracts.IntegrationEvents;

namespace MUEats.Restaurants.Application.IntegrationEvents;

public class OrderRejectedEvent : IntegrationEvent
{
    public Guid OrderId { get; init; }
    
    public string Reason { get; init; }
}