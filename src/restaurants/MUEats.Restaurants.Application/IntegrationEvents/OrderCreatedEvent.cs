using MUEats.Restaurants.Application.DTOs;

namespace MUEats.Restaurants.Application.IntegrationEvents;

public class OrderCreatedEvent : IntegrationEvent
{
    public Guid OrderId { get; set; }
    
    public OrderDto Dto { get; set; }
}