using MUEats.Restaurants.Application.DTOs;
using SharedContracts.IntegrationEvents;

namespace MUEats.Restaurants.Application.IntegrationEvents;

public class OrderCreatedEvent : IntegrationEvent
{
    public Guid OrderId { get; init; }
    
    public OrderDto Dto { get; init; }
}