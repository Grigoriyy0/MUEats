using MUEats.Application.Dto.ShoppingCart;
using MUEats.Core.Domain.ShoppingCart;
using MUEats.Core.Domain.ShoppingCart.ValueObjects;

namespace MUEats.Application.Ports;

public interface IShoppingCartsRepository
{
    Task AddAsync(ShoppingCart shoppingCart, CancellationToken ct);

    Task AddCartItemAsync(CartItem cartItem, CancellationToken ct);
    
    Task<ShoppingCart?> GetByIdAsync(Guid id, CancellationToken ct);

    Task<ShoppingCart?> GetByUserIdAsync(Guid userId, CancellationToken ct);

    Task<CartDto?> GetCartDtoAsync(Guid userId, CancellationToken ct);

    Task<CartItem?> GetCartItemAsync(Guid cartItemId, CancellationToken ct);
    
    Task DeleteAsync(ShoppingCart shoppingCart, CancellationToken ct);

    Task DeleteCartItemAsync(CartItem cartItem, CancellationToken ct);
    
    Task UpdateAsync(ShoppingCart shoppingCart, CancellationToken ct);

    Task UpdateCartItemAsync(CartItem cartItem, CancellationToken ct);

    Task ClearCartAsync(Guid cartId, CancellationToken ct);
}