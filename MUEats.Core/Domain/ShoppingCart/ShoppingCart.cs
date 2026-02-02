using CSharpFunctionalExtensions;
using MUEats.Core.Domain.ShoppingCart.ValueObjects;

namespace MUEats.Core.Domain.ShoppingCart;

public class ShoppingCart
{
    public ShoppingCart()
    {
        
    }
    
    private ShoppingCart(Guid userId, Guid restaurantId)
    {
        Id = Guid.NewGuid();
        UserId = userId;
        RestaurantId = restaurantId;
    }

    public Guid Id { get; init; }
    
    public Guid UserId { get; init; }
    
    public Guid RestaurantId { get; init; }

    public List<CartItem> CartItems { get; private set; } = [];

    public static Result<ShoppingCart> Create(string name, Guid userId, Guid restaurantId)
    {
        return new ShoppingCart(userId, restaurantId);
    }
}