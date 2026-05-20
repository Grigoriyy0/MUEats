using MUEats.Application.Dto.ShoppingCart;
using MUEats.Application.Ports;
using MUEats.Core.Domain.ShoppingCart;
using MUEats.Core.Domain.ShoppingCart.ValueObjects;

namespace MUEats.Application.Services;

public class ShoppingCartsService(IShoppingCartsRepository shoppingCartsRepository, 
    IUnitOfWork uow,
    ICurrentUserContext currentUserContext)
{
    public async Task AddToCartAsync(AddFoodItemDto dto, CancellationToken ct)
    {
        try
        {
            await uow.BeginTransactionAsync(ct);

            var userId = currentUserContext.GetUserId();
            
            var cart = await shoppingCartsRepository.GetByUserIdAsync(userId, ct);

            cart = await EnsureShoppingCartAsync(cart, userId, dto.RestaurantId, dto.RestaurantName, ct);
            
            await AddOrIncreaseItem(dto, cart, ct);
            
            await uow.SaveChangesAsync(ct);
            await uow.CommitTransactionAsync(ct);
        }
        catch (Exception)
        {
            await uow.RollbackTransactionAsync(ct);
            throw;
        }
    }

    public async Task DeleteCartItemAsync(Guid cartItemId, CancellationToken ct)
    {
        try
        {
            await uow.BeginTransactionAsync(ct);

            var cartItem = await shoppingCartsRepository.GetCartItemAsync(cartItemId, ct);

            if (cartItem is null)
            {
                throw new ArgumentException("No such cart item");
            }

            await DeleteOrDecreaseItem(cartItem, ct);
            
            await uow.SaveChangesAsync(ct);
            await uow.CommitTransactionAsync(ct);
        }
        catch (Exception)
        {
            await uow.RollbackTransactionAsync(ct);
            throw;
        }
    }

    private async Task AddOrIncreaseItem(AddFoodItemDto dto, 
        ShoppingCart cart, 
        CancellationToken ct)
    {
        var existingItem = cart.CartItems.FirstOrDefault(x => x.FoodItemId == dto.ItemId);

        if (existingItem != null)
        {
            existingItem.Quantity++;
            await shoppingCartsRepository.UpdateCartItemAsync(existingItem, ct);
            return;
        }
        
        var newCartItem = new CartItem
        {
            Id = Guid.NewGuid(),
            CartId = cart.Id,
            FoodItemId = dto.ItemId,
            Name = dto.ItemName,
            Price = dto.ItemPrice,
            Quantity = 1
        };

        await shoppingCartsRepository.AddCartItemAsync(newCartItem, ct);
        
        cart.CartItems.Add(newCartItem);
    }

    private async Task DeleteOrDecreaseItem(CartItem cartItem, CancellationToken ct)
    {
        if (cartItem.Quantity > 1)
        {
            cartItem.Quantity--;
            await shoppingCartsRepository.UpdateCartItemAsync(cartItem, ct);
            return;
        }

        await shoppingCartsRepository.DeleteCartItemAsync(cartItem, ct);
    }

    private async Task<ShoppingCart> EnsureShoppingCartAsync(ShoppingCart? cart, 
        Guid userId, 
        Guid restaurantId, 
        string restaurantName,
        CancellationToken ct)
    {
        if (cart != null && cart.RestaurantId == restaurantId)
        {
            return cart;
        }

        var newCart = new ShoppingCart
        {
            Id = Guid.NewGuid(),
            UserId = userId,
            RestaurantId = restaurantId,
            RestaurantName = restaurantName
        };
        
        await shoppingCartsRepository.AddAsync(newCart, ct);
        
        return newCart;
    }

    public Task<CartDto?> GetShoppingCartAsync(Guid userId, CancellationToken ct)
    {
        return shoppingCartsRepository.GetCartDtoAsync(userId, ct);
    }
}