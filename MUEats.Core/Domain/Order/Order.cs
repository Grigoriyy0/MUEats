using MUEats.Core.Domain.Order.ValueObjects;
using MUEats.Core.Primitives;

namespace MUEats.Core.Domain.Order;

public class Order
{
    public Guid Id  { get; set; }
    
    public Money Price { get; set; }
    
    public Address Address { get; set; }
    
    public Guid UserId { get; set; }
    
    public Guid RestaurantId { get; set; }
    
    public Guid CourierId { get; set; }
    
    public DateTime OrderDate { get; set; }
    
    public DateTime DeliverBefore { get; set; }
    
    public DateTime DeliveredAt { get; set; }

    public List<OrderItem> OrderItems { get; set; } = [];
}