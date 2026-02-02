using MUEats.Core.Primitives;
using MUEats.Core.Primitives.ValueObjects;

namespace MUEats.Core.Domain.Order.ValueObjects;

public class OrderItem
{
    public OrderItem()
    {
        
    }
    
    private OrderItem(Guid cartItemId, string name, Money price, int quantity)
    {
        CartItemId = cartItemId;
        Name = name;
        Price = price;
        Quantity = quantity;
    }
    
    public Guid CartItemId { get; init; }

    public string Name { get; init; } = null!;

    public Money Price { get; init; } = null!;
    
    public int Quantity { get; private set; }

    public static OrderItem Create(Guid cartItemId, string name, Money price, int quantity)
    {
        return new OrderItem(cartItemId, name, price, quantity);
    }
}