using SharedContracts.DTOs;

namespace SharedContracts.IntegrationEvents;

public class OrderCreatedEvent : IntegrationEvent
{
    public Guid OrderId { get; init; }
    
    public OrderDto Dto { get; init; }
}