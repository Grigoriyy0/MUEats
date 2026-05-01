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

        var restaurantExists = await _restaurantsRepository.AnyAsync(restaurantId, ct);

        if (!restaurantExists)
        {
            await _unitOfWork.RollbackTransactionAsync(ct);
            return ApplicationErrors.Restaurant.RestaurantNotFound;
        }

        var menuResult = Menu.Create(restaurantId);

        await _menusRepository.AddAsync(menuResult.Value, ct);

        await _unitOfWork.SaveChangesAsync(ct);
        await _unitOfWork.CommitTransactionAsync(ct);
        
        return UnitResult.Success<Error>();
    }

    public async Task<UnitResult<Error>> AddMenuItemAsync(CreateMenuItemDto dto, CancellationToken ct)
    {
        await  _unitOfWork.BeginTransactionAsync(ct);
        
        var menu = await _menusRepository.GetByIdAsync(dto.MenuId, ct);

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

    public async Task<UnitResult<Error>> AddCategoryAsync(CreateCategoryDto dto, CancellationToken ct)
    {
        await _unitOfWork.BeginTransactionAsync(ct);
        
        var menu = await _menusRepository.GetByIdAsync(dto.MenuId, ct);

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
}