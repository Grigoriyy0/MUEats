using Microsoft.EntityFrameworkCore;
using MUEats.Application.Dto.ShoppingCart;
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
        return context.ShoppingCarts
            .Include(x => x.CartItems)
            .FirstOrDefaultAsync(x => x.UserId == userId, ct);
    }

    public Task<CartDto?> GetCartDtoAsync(Guid userId, CancellationToken ct)
    {
        return context.ShoppingCarts
            .Where(x => x.UserId == userId)
            .Select(c => new CartDto
            {
                Id = c.Id,
                UserId = c.UserId,
                RestaurantId = c.RestaurantId,
                RestaurantName = c.RestaurantName,
                Items = c.CartItems.Select(ci => new CartItemDto
                {
                    Id = ci.Id,
                    CartId = ci.CartId,
                    FoodItemId = ci.FoodItemId,
                    FoodItemName = ci.Name,
                    Price = ci.Price,
                    Quantity = ci.Quantity
                }).ToList()
            }).FirstOrDefaultAsync(ct);
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

    public Task UpdateCartItemAsync(CartItem cartItem, CancellationToken ct)
    {
        context.CartItems.Update(cartItem);
        return Task.CompletedTask;
    }

    public Task<CartItem?> GetCartItemAsync(Guid cartItemId, CancellationToken ct)
    {
        return context.CartItems
            .FirstOrDefaultAsync(x => x.Id == cartItemId, ct);
    }

    public Task DeleteCartItemAsync(CartItem cartItem, CancellationToken ct)
    {
        context.Remove(cartItem);
        return Task.CompletedTask;
    }
    
    public Task AddCartItemAsync(CartItem cartItem, CancellationToken ct)
    {
        return context.CartItems.AddAsync(cartItem, ct)
            .AsTask();
    }

    public async Task ClearCartAsync(Guid cartId, CancellationToken ct)
    {
        var cartItems = await context.CartItems.Where(x => x.CartId == cartId)
            .ToListAsync(ct);
        
        context.RemoveRange(cartItems);
    }
}
