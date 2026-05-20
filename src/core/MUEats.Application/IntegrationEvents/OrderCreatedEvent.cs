
using MUEats.Application.Dto.Order;

namespace MUEats.Application.IntegrationEvents;

public class OrderCreatedEvent : IntegrationEvent
{
    public Guid OrderId { get; set; }
    
    public OrderDto Dto { get; set; }
}