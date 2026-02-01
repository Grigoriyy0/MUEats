using MUEats.Core.Primitives;

namespace MUEats.Core.Domain.Order.ValueObjects;

public class OrderItem
{
    public Guid CartItemId { get; set; }
    
    public string Name { get; set; }
    
    public Money Price { get; set; }
    
    public int Quantity { get; set; }
}