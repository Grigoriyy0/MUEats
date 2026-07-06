using CSharpFunctionalExtensions;
using MUEats.Application.Dto.ShoppingCart;
using MUEats.Application.Ports;
using MUEats.Core.Domain.ShoppingCart;
using MUEats.Core.Domain.ShoppingCart.ValueObjects;
using Primitives;

namespace MUEats.Application.Services;

public class ShoppingCartsService(
    IShoppingCartsRepository shoppingCartsRepository, 
    IUnitOfWork uow,
    ICurrentUserContext currentUserContext)
{
    public async Task<UnitResult<Error>> AddToCartAsync(AddFoodItemDto dto, CancellationToken ct)
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

            return UnitResult.Success<Error>();
        }
        catch (Exception)
        {
            await uow.RollbackTransactionAsync(ct);
            throw; 
        }
    }

    public async Task<UnitResult<Error>> DeleteCartItemAsync(Guid cartItemId, CancellationToken ct)
    {
        try
        {
            await uow.BeginTransactionAsync(ct);

            var cartItem = await shoppingCartsRepository.GetCartItemAsync(cartItemId, ct);

            if (cartItem is null)
            {
                await uow.RollbackTransactionAsync(ct);
                return ApplicationErrors.ShoppingCart.ItemNotFound;
            }

            await DeleteOrDecreaseItem(cartItem, ct);
            
            await uow.SaveChangesAsync(ct);
            await uow.CommitTransactionAsync(ct);

            return UnitResult.Success<Error>();
        }
        catch (Exception)
        {
            await uow.RollbackTransactionAsync(ct);
            throw;
        }
    }

    public async Task<Result<CartDto, Error>> GetShoppingCartAsync(Guid userId, CancellationToken ct)
    {
        var cartDto = await shoppingCartsRepository.GetCartDtoAsync(userId, ct);
        
        if (cartDto is null)
        {
            return ApplicationErrors.ShoppingCart.CartNotFound;
        }

        return cartDto;
    }

    private async Task AddOrIncreaseItem(AddFoodItemDto dto, ShoppingCart cart, CancellationToken ct)
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

    private async Task<ShoppingCart> EnsureShoppingCartAsync(
        ShoppingCart? cart, 
        Guid userId, 
        Guid restaurantId, 
        string restaurantName,
        CancellationToken ct)
    {
        if (cart != null && cart.RestaurantId == restaurantId)
        {
            return cart;
        }
        
        if (cart != null)
        {
            await shoppingCartsRepository.DeleteAsync(cart, ct);
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
}