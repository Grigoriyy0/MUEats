namespace MUEats.Core.Domain.Events.Order;

public class OrderPickedUpEvent
{
    public Guid OrderId { get; set; }
}