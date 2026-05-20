namespace MUEats.Core.Domain.ShoppingCart.ValueObjects;

public class CartItem
{
    public Guid Id { get; init; }
    
    public Guid FoodItemId { get; init; }

    public Guid CartId { get; init; }

    public string Name { get; set; }
    
    public decimal Price { get; init; }

    public int Quantity { get; set; }

    public ShoppingCart? Cart { get; set; }
}