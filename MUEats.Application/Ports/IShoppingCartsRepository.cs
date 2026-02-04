using MUEats.Core.Domain.ShoppingCart;

namespace MUEats.Application.Ports;

public interface IShoppingCartsRepository
{
    Task AddAsync(ShoppingCart shoppingCart, CancellationToken ct);
    
    Task<ShoppingCart?> GetByIdAsync(Guid id, CancellationToken ct);
    
    Task DeleteAsync(ShoppingCart shoppingCart, CancellationToken ct);
    
    Task UpdateAsync(ShoppingCart shoppingCart, CancellationToken ct);
}