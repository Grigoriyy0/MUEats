using MUEats.Core.Primitives;

namespace MUEats.Core.Domain.ShoppingCart.ValueObjects;

public class CartItem
{
    public Guid FoodItemId { get; set; }
    
    public Guid CartId { get; set; }
    
    public Money Price { get; set; }
    
    public int Quantity { get; set; }
    
    public ShoppingCart? Cart { get; set; }
}