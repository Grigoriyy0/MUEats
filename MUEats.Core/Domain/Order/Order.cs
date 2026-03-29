using CSharpFunctionalExtensions;
using MUEats.Core.Domain.Order.ValueObjects; 

namespace MUEats.Core.Domain.Order;

public class Order
{
    public Guid Id  { get; init; }

    public decimal TotalPrice { get; init; }

    public string Address { get; set; } = null!;
    
    public OrderStatus OrderStatus { get; set; }
    
    public Guid UserId { get; init; }
    
    public Guid RestaurantId { get; init; }

    public DateTime OrderDate { get; init; }
    
    public List<OrderItem> OrderItems { get; set; } = [];
}