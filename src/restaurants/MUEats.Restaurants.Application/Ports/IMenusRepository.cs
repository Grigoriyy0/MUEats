using MUEats.Restaurants.Application.DTOs;
using MUEats.Restaurants.Core.Domain.Menu;

namespace MUEats.Restaurants.Application.Ports;

public interface IMenusRepository
{
    Task<Menu?> GetByIdAsync(Guid id, CancellationToken ct);
    
    Task<MenuDto?> GetDtoByIdAsync(Guid restaurantId, CancellationToken ct);

    Task<MenuItemDetailsDto?> GetMenuItemDto(Guid menuId, Guid itemId, CancellationToken ct);
    
    Task AddAsync(Menu menu, CancellationToken ct);
    
    Task UpdateAsync(Menu menu, CancellationToken ct);
    
    Task DeleteAsync(Menu menu, CancellationToken ct);
    
    Task<bool> AnyAsync(Guid id, CancellationToken ct);
}