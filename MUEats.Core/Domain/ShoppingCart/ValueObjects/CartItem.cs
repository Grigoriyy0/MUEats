using CSharpFunctionalExtensions;
using MUEats.Core.Primitives;
using MUEats.Core.Primitives.ValueObjects;

namespace MUEats.Core.Domain.ShoppingCart.ValueObjects;

public class CartItem
{
    public CartItem()
    {
        
    }
    
    private CartItem(Guid foodItemId, Guid cartId, Money price, int quantity)
    {
        FoodItemId = foodItemId;
        CartId = cartId;
        Price = price;
        Quantity = quantity;
    }

    public Guid FoodItemId { get; init; }
    
    public Guid CartId { get; init; }
    
    public Money Price { get; init; }
    
    public int Quantity { get; private set; }
    
    public ShoppingCart? Cart { get; private set; }

    public static Result<CartItem> Create(Guid foodItemId, Guid cartId, Money price, int quantity)
    {
        return new CartItem(foodItemId, cartId, price, quantity);
    }
}