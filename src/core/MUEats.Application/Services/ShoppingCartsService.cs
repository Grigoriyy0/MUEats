using CSharpFunctionalExtensions;
using MUEats.Application.Dto.ShoppingCart;
using MUEats.Application.Ports;
using MUEats.Core.Domain.ShoppingCart;
using MUEats.Core.Domain.ShoppingCart.ValueObjects;
using Primitives;

namespace MUEats.Application.Services;

public class ShoppingCartsService
{
    private readonly IShoppingCartsRepository _shoppingCartsRepository;
    private readonly IUnitOfWork _uow;
    private readonly ICurrentUserContext _currentUserContext;

    public ShoppingCartsService(IShoppingCartsRepository shoppingCartsRepository, 
        IUnitOfWork uow,
        ICurrentUserContext currentUserContext)
    {
        _shoppingCartsRepository = shoppingCartsRepository;
        _uow = uow;
        _currentUserContext = currentUserContext;
    }

    public async Task<UnitResult<Error>> AddToCartAsync(AddFoodItemDto dto, CancellationToken ct)
    {
        try
        {
            await _uow.BeginTransactionAsync(ct);

            var userId = _currentUserContext.GetUserId();
            var cart = await _shoppingCartsRepository.GetByUserIdAsync(userId, ct);

            cart = await EnsureShoppingCartAsync(cart, userId, dto.RestaurantId, dto.RestaurantName, ct);
            
            await AddOrIncreaseItem(dto, cart, ct);
            
            await _uow.SaveChangesAsync(ct);
            await _uow.CommitTransactionAsync(ct);

            return UnitResult.Success<Error>();
        }
        catch (Exception)
        {
            await _uow.RollbackTransactionAsync(ct);
            throw; 
        }
    }

    public async Task<UnitResult<Error>> DeleteCartItemAsync(Guid cartItemId, CancellationToken ct)
    {
        try
        {
            await _uow.BeginTransactionAsync(ct);

            var cartItem = await _shoppingCartsRepository.GetCartItemAsync(cartItemId, ct);

            if (cartItem is null)
            {
                await _uow.RollbackTransactionAsync(ct);
                return ApplicationErrors.ShoppingCart.ItemNotFound;
            }

            await DeleteOrDecreaseItem(cartItem, ct);
            
            await _uow.SaveChangesAsync(ct);
            await _uow.CommitTransactionAsync(ct);

            return UnitResult.Success<Error>();
        }
        catch (Exception)
        {
            await _uow.RollbackTransactionAsync(ct);
            throw;
        }
    }

    public async Task<Result<CartDto, Error>> GetShoppingCartAsync(Guid userId, CancellationToken ct)
    {
        var cartDto = await _shoppingCartsRepository.GetCartDtoAsync(userId, ct);
        
        if (cartDto is null)
        {
            return ApplicationErrors.ShoppingCart.CartNotFound;
        }

        return cartDto;
    }

    private async Task AddOrIncreaseItem(AddFoodItemDto dto, 
        ShoppingCart cart, 
        CancellationToken ct)
    {
        var existingItem = cart.CartItems.FirstOrDefault(x => x.FoodItemId == dto.ItemId);

        if (existingItem != null)
        {
            existingItem.Quantity++;
            await _shoppingCartsRepository.UpdateCartItemAsync(existingItem, ct);
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

        await _shoppingCartsRepository.AddCartItemAsync(newCartItem, ct);
        cart.CartItems.Add(newCartItem);
    }

    private async Task DeleteOrDecreaseItem(CartItem cartItem, CancellationToken ct)
    {
        if (cartItem.Quantity > 1)
        {
            cartItem.Quantity--;
            await _shoppingCartsRepository.UpdateCartItemAsync(cartItem, ct);
            return;
        }

        await _shoppingCartsRepository.DeleteCartItemAsync(cartItem, ct);
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
        
        if (cart != null)
        {
            await _shoppingCartsRepository.DeleteAsync(cart, ct);
        }

        var newCart = new ShoppingCart
        {
            Id = Guid.NewGuid(),
            UserId = userId,
            RestaurantId = restaurantId,
            RestaurantName = restaurantName
        };
        
        await _shoppingCartsRepository.AddAsync(newCart, ct);
        return newCart;
    }
}