using MUEats.Core.Domain.ShoppingCart.ValueObjects;

namespace MUEats.Core.Domain.ShoppingCart;

public class ShoppingCart
{
    public Guid Id { get; init; }
    
    public Guid UserId { get; init; }
    
    public Guid RestaurantId { get; init; }
    
    public string RestaurantName { get; set; }

    public List<CartItem> CartItems { get; set; } = [];
}