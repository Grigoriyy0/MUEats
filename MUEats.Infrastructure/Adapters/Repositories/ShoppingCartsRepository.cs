using Microsoft.EntityFrameworkCore;
using MUEats.Application.Ports;
using MUEats.Core.Domain.ShoppingCart;
using MUEats.Core.Domain.ShoppingCart.ValueObjects;
using MUEats.Infrastructure.Persistence;

namespace MUEats.Infrastructure.Adapters.Repositories;

public class ShoppingCartsRepository(MueDbContext context) : IShoppingCartsRepository
{
    public Task AddAsync(ShoppingCart shoppingCart, CancellationToken ct)
    {
        return context.ShoppingCarts.AddAsync(shoppingCart, ct)
            .AsTask();
    }

    public Task<ShoppingCart?> GetByIdAsync(Guid id, CancellationToken ct)
    {
        return context.ShoppingCarts.Where(x => x.Id == id)
            .Include(x => x.CartItems)
            .FirstOrDefaultAsync(ct);
    }

    public Task<ShoppingCart?> GetByUserIdAsync(Guid userId, CancellationToken ct)
    {
        return context.ShoppingCarts.FirstOrDefaultAsync(x => x.UserId == userId, ct);
    }
    
    public Task DeleteAsync(ShoppingCart shoppingCart, CancellationToken ct)
    {
        context.ShoppingCarts.Remove(shoppingCart);
        return Task.CompletedTask;
    }

    public Task UpdateAsync(ShoppingCart shoppingCart, CancellationToken ct)
    {
        context.ShoppingCarts.Update(shoppingCart);
        return Task.CompletedTask;
    }
    
    public Task AddCartItemAsync(CartItem cartItem, CancellationToken ct)
    {
        return context.CartItems.AddAsync(cartItem, ct)
            .AsTask();
    }
}
