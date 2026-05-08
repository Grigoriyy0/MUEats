using MUEats.Core.Domain.Events;

namespace MUEats.Infrastructure.IntegrationEvents;

public class OrderSentEvent : IntegrationEvent
{
    public Guid OrderId { get; set; }
    
    public Guid RestaurantId { get; set; }
}