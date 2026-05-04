using CSharpFunctionalExtensions;
using MUEats.Restaurants.Application.DTOs;
using MUEats.Restaurants.Application.Ports;
using MUEats.Restaurants.Core.Domain.Menu;
using Primitives;

namespace MUEats.Restaurants.Application.Services;

public class MenusService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IRestaurantsRepository _restaurantsRepository;
    private readonly IMenusRepository _menusRepository;

    public MenusService(IUnitOfWork unitOfWork, 
        IRestaurantsRepository restaurantsRepository, 
        IMenusRepository menusRepository)
    {
        _unitOfWork = unitOfWork;
        _restaurantsRepository = restaurantsRepository;
        _menusRepository = menusRepository;
    }

    public async Task<UnitResult<Error>> CreateAsync(Guid restaurantId, CancellationToken ct)
    {
        await _unitOfWork.BeginTransactionAsync(ct);

        var restaurant = await _restaurantsRepository.GetByIdAsync(restaurantId, ct);

        if (restaurant is null)
        {
            await _unitOfWork.RollbackTransactionAsync(ct);
            return ApplicationErrors.Restaurant.RestaurantNotFound;
        }

        var menuExists = await _menusRepository.AnyAsync(restaurantId, ct);

        if (menuExists)
        {
            await _unitOfWork.RollbackTransactionAsync(ct);
            return ApplicationErrors.Menu.MenuAlreadyExists;
        }
        
        var menuResult = Menu.Create(restaurantId);
        
        restaurant.AddMenuId(menuResult.Value.Id);
        
        await _menusRepository.AddAsync(menuResult.Value, ct);
        
        await _unitOfWork.SaveChangesAsync(ct);
        await _unitOfWork.CommitTransactionAsync(ct);
        
        return UnitResult.Success<Error>();
    }

    public async Task<UnitResult<Error>> AddMenuItemAsync(Guid menuId,
        CreateMenuItemDto dto, 
        CancellationToken ct)
    {
        await  _unitOfWork.BeginTransactionAsync(ct);
        
        var menu = await _menusRepository.GetByIdAsync(menuId, ct);

        if (menu == null)
        {
            await _unitOfWork.RollbackTransactionAsync(ct);
            return ApplicationErrors.Menu.MenuNotFound;
        }
        
        var menuItemResult = menu.AddMenuItem(dto.ItemName, dto.ItemDescription, dto.ItemPrice, dto.CategoryId);

        if (menuItemResult.IsFailure)
        {
            await  _unitOfWork.RollbackTransactionAsync(ct);
            return menuItemResult.Error;
        }
        
        await _unitOfWork.SaveChangesAsync(ct);
        await _unitOfWork.CommitTransactionAsync(ct);
        
        return UnitResult.Success<Error>();
    }

    public async Task<UnitResult<Error>> AddCategoryAsync(Guid menuId, CreateCategoryDto dto, CancellationToken ct)
    {
        await _unitOfWork.BeginTransactionAsync(ct);
        
        var menu = await _menusRepository.GetByIdAsync(menuId, ct);

        if (menu == null)
        {
            await _unitOfWork.RollbackTransactionAsync(ct);
            return ApplicationErrors.Menu.MenuNotFound;
        }
        
        var categoryResult = menu.AddCategory(dto.Name, dto.Description);

        if (categoryResult.IsFailure)
        {
            await  _unitOfWork.RollbackTransactionAsync(ct);
            return categoryResult.Error;
        }
        
        await _unitOfWork.SaveChangesAsync(ct);
        await _unitOfWork.CommitTransactionAsync(ct);
        
        return UnitResult.Success<Error>();
    }

    public async Task<UnitResult<Error>> AddOptionsGroupAsync(Guid menuId,
        Guid itemId,
        CreateOptionsGroupDto dto,
        CancellationToken ct)
    {
        await _unitOfWork.BeginTransactionAsync(ct);
        
        var menu = await _menusRepository.GetByIdAsync(menuId, ct);

        if (menu == null)
        {
            await _unitOfWork.RollbackTransactionAsync(ct);
            return ApplicationErrors.Menu.MenuNotFound;
        }

        var result = menu.AddOptionsGroup(itemId, dto.Name, dto.Description);

        if (result.IsFailure)
        {
            await  _unitOfWork.RollbackTransactionAsync(ct);
            return result.Error;
        }
        
        await _unitOfWork.SaveChangesAsync(ct);
        await _unitOfWork.CommitTransactionAsync(ct);
        
        return UnitResult.Success<Error>();
    }

    public async Task<UnitResult<Error>> DeleteOptionsGroupAsync(Guid menuId, 
        Guid groupId,
        CancellationToken ct)
    {
        await _unitOfWork.BeginTransactionAsync(ct);

        var menu = await _menusRepository.GetByIdAsync(menuId, ct);

        if (menu is null)
        {
            await _unitOfWork.RollbackTransactionAsync(ct);
            return ApplicationErrors.Menu.MenuNotFound;
        }

        var result = menu.DeleteOptionsGroup(groupId);

        if (result.IsFailure)
        {
            await _unitOfWork.RollbackTransactionAsync(ct);
            return result.Error;
        }
        
        await _unitOfWork.SaveChangesAsync(ct);
        await _unitOfWork.CommitTransactionAsync(ct);
        
        return UnitResult.Success<Error>();
    }

    public async Task<UnitResult<Error>> DeleteItemOptionAsync(Guid menuId, 
        Guid itemId,
        CancellationToken ct)
    {
        await _unitOfWork.BeginTransactionAsync(ct);

        var menu = await _menusRepository.GetByIdAsync(menuId, ct);

        if (menu is null)
        {
            await _unitOfWork.RollbackTransactionAsync(ct);
            return ApplicationErrors.Menu.MenuNotFound;
        }

        var result = menu.DeleteItemOption(itemId);

        if (result.IsFailure)
        {
            await _unitOfWork.RollbackTransactionAsync(ct);
            return result.Error;
        }

        await _unitOfWork.SaveChangesAsync(ct);
        await _unitOfWork.CommitTransactionAsync(ct);
        
        return UnitResult.Success<Error>();
    }

    public async Task<UnitResult<Error>> AddItemOptionAsync(Guid menuId,
        Guid groupId,
        AddItemOptionDto dto,
        CancellationToken ct)
    {
        await  _unitOfWork.BeginTransactionAsync(ct);
        
        var menu = await _menusRepository.GetByIdAsync(menuId, ct);

        if (menu == null)
        {
            await _unitOfWork.RollbackTransactionAsync(ct);
            return ApplicationErrors.Menu.MenuNotFound;
        }
        
        var result = menu.AddItemOption(groupId, dto.Value, dto.AdditionalPrice);

        if (result.IsFailure)
        {
            await  _unitOfWork.RollbackTransactionAsync(ct);
            return result.Error;
        }
        
        await _unitOfWork.SaveChangesAsync(ct);
        await _unitOfWork.CommitTransactionAsync(ct);
        
        return UnitResult.Success<Error>();
    }
    
    public async Task<UnitResult<Error>> UpdateMenuItemAsync(Guid menuId, 
        Guid itemId, 
        UpdateMenuItemDto dto, 
        CancellationToken ct)
    {
        await _unitOfWork.BeginTransactionAsync(ct);
        
        var menu = await _menusRepository.GetByIdAsync(menuId, ct);

        if (menu == null)
        {
            return ApplicationErrors.Menu.MenuNotFound;
        }
        
        var updateResult = menu.UpdateMenuItem(itemId, 
            dto.ItemName, 
            dto.ItemDescription, 
            dto.ItemPrice, 
            dto.IsAvailable, 
            dto.CategoryId);

        if (updateResult.IsFailure)
        {
            await  _unitOfWork.RollbackTransactionAsync(ct);
            return updateResult.Error;
        }
        
        await _unitOfWork.SaveChangesAsync(ct);
        await _unitOfWork.CommitTransactionAsync(ct);
        
        return UnitResult.Success<Error>();
    }
}