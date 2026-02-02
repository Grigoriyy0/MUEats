using CSharpFunctionalExtensions;
using MUEats.Core.Domain.Order.ValueObjects; 

namespace MUEats.Core.Domain.Order;

public class Order
{
    public Guid Id  { get; init; }

    public decimal Price { get; init; }

    public string Address { get; set; } = null!;
    
    public OrderStatus Status { get; set; }
    
    public Guid UserId { get; init; }
    
    public Guid RestaurantId { get; init; }
    
    public Guid CourierId { get; init; }
    
    public DateTime OrderDate { get; init; }
    
    public DateTime DeliverBefore { get; set; }
    
    public DateTime DeliveredAt { get; init; }

    public List<OrderItem> OrderItems { get; set; } = [];

}