using MUEats.Core.Domain.Order.ValueObjects; 

namespace MUEats.Core.Domain.Order;

public class Order
{
    public Guid Id  { get; init; }

    public Guid UserId { get; init; }
    
    public Guid RestaurantId { get; init; }
    
    public DateTime? PickupTime { get; set; }
    
    public decimal TotalPrice { get; init; }
    
    public OrderStatus Status { get; set; }
    
    public DateTime CreatedAt { get; init; }
    
    public string? RejectReason { get; set; }
    
    public List<OrderItem> OrderItems { get; set; } = [];
}