using MUEats.Application.Ports;
using MUEats.Core.Domain.ShoppingCart;
using MUEats.Core.Domain.ShoppingCart.ValueObjects;

namespace MUEats.Application.Services;

public class ShoppingCartsService(
    IShoppingCartsRepository shoppingCartsRepository, 
    IFoodItemsRepository foodItemsRepository,
    IUsersRepository usersRepository,
    IUnitOfWork uow
    )
{
    public async Task AddToCartAsync(Guid userId, Guid foodItemId, CancellationToken ct)
    {
        await uow.BeginTransactionAsync(ct);
        
        var foodItem  = await foodItemsRepository.GetByIdAsync(foodItemId, ct);

        if (foodItem == null)
        {
            throw new ArgumentException($"Food item with id {foodItemId} does not exist");
        }

        if (!foodItem.IsAvailable)
        {
            throw new ArgumentException($"Food item with id {foodItemId} can not be ordered now");
        }
        
        var cart = await shoppingCartsRepository.GetByUserIdAsync(userId, ct);
        
        if (cart == null || cart.RestaurantId != foodItem.RestaurantId)
        {
            var checkUser = await usersRepository.AnyAsync(userId, ct);

            if (!checkUser)
            {
                throw new ArgumentException($"User with id {userId} does not exist");
            }
            
            var newCart = new ShoppingCart
            {
                Id = Guid.NewGuid(),
                RestaurantId = foodItem.RestaurantId,
                UserId = userId,
            };

            var cartItem = new CartItem
            {
                Id = Guid.NewGuid(),
                FoodItemId = foodItemId,
                CartId = newCart.Id,
                Price = foodItem.Price,
                Quantity = 1,
            };
            
            await shoppingCartsRepository.AddAsync(newCart, ct);
            await shoppingCartsRepository.AddCartItemAsync(cartItem, ct);
            
            return;
        }

        var newCartItem = new CartItem
        {
            Id = Guid.NewGuid(),
            FoodItemId = foodItemId,
            Price = foodItem.Price,
            CartId = cart.Id,
            Quantity = 1,
        };
        
        await shoppingCartsRepository.AddCartItemAsync(newCartItem, ct);
        
        await uow.SaveChangesAsync(ct);
        await uow.CommitTransactionAsync(ct);
    }
}