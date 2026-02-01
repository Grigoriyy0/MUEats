using MUEats.Core.Domain.ShoppingCart.ValueObjects;

namespace MUEats.Core.Domain.ShoppingCart;

public class ShoppingCart
{
    public Guid Id { get; set; }
    
    public Guid UserId { get; set; }
    
    public Guid RestaurantId { get; set; }

    public List<CartItem> CartItems { get; set; } = [];
}