using Microsoft.EntityFrameworkCore;
using MUEats.Core.Domain.ShoppingCart;
using MUEats.Infrastructure.Persistence;

namespace MUEats.Infrastructure.Adapters.Repositories;

public class ShoppingCartsRepository(MueDbContext context)
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
}
