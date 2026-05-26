using MUEats.Restaurants.Application.DTOs;
using MUEats.Restaurants.Application.Ports;

namespace MUEats.Restaurants.Application.Services;

public class MenusQueries
{
    private readonly IMenusRepository _repository;

    public MenusQueries(IMenusRepository repository)
    {
        _repository = repository;
    }
    
    public Task<MenuDto?> GetDtoByIdAsync(Guid restaurantId, CancellationToken ct)
    {
        return _repository.GetDtoByIdAsync(restaurantId, false, ct);
    }
    
    public Task<MenuItemDetailsDto?> GetItemDtoAsync(Guid menuId,
        Guid itemId,
        CancellationToken ct)
    {
        return _repository.GetMenuItemDto(menuId, itemId, ct);
    }

    public Task<MenuDto?> GetAdminViewDtoByIdAsync(Guid restaurantId, CancellationToken ct)
    {
        return _repository.GetDtoByIdAsync(restaurantId, true, ct);
    }
}