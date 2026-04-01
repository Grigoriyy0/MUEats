using MUEats.Application.Dto.ShoppingCart;
using MUEats.Application.Ports;
using MUEats.Core.Domain.Restaurant.Entities;
using MUEats.Core.Domain.ShoppingCart;
using MUEats.Core.Domain.ShoppingCart.ValueObjects;

namespace MUEats.Application.Services;

public class ShoppingCartsService(
    IShoppingCartsRepository shoppingCartsRepository,
    IFoodItemsRepository foodItemsRepository,
    IUnitOfWork uow
)
{
    public async Task AddToCartAsync(AddFoodItemDto dto, CancellationToken ct)
    {
        try
        {
            await uow.BeginTransactionAsync(ct);

            var foodItem = await foodItemsRepository.GetByIdAsync(dto.FoodItemId, ct);

            if (foodItem == null) throw new ArgumentException($"Food item with id {dto.FoodItemId} does not exist");

            if (!foodItem.IsAvailable) throw new ArgumentException($"Food item with id {dto.FoodItemId} can not be ordered now");

            var cart = await shoppingCartsRepository.GetByUserIdAsync(dto.UserId, ct);

            cart = await EnsureShoppingCartAsync(cart, dto.UserId, foodItem.RestaurantId, ct);
            
            await AddOrIncreaseItem(cart, foodItem, ct);
            
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

    private async Task AddOrIncreaseItem(ShoppingCart cart, FoodItem foodItem, CancellationToken ct)
    {
        var existingItem = cart.CartItems.FirstOrDefault(x => x.FoodItemId == foodItem.Id);

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
            FoodItemId = foodItem.Id,
            Price = foodItem.Price,
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

    private async Task<ShoppingCart> EnsureShoppingCartAsync(ShoppingCart? cart, Guid userId, Guid restaurantId, CancellationToken ct)
    {
        if (cart != null && cart.RestaurantId == restaurantId)
        {
            return cart;
        }

        var newCart = new ShoppingCart
        {
            Id = Guid.NewGuid(),
            UserId = userId,
            RestaurantId = restaurantId
        };
        
        await shoppingCartsRepository.AddAsync(newCart, ct);
        
        return newCart;
    }

    public async Task<CartDto?> GetShoppingCartAsync(Guid userId, CancellationToken ct)
    {
        return await shoppingCartsRepository.GetCartDtoAsync(userId, ct);
    }
}