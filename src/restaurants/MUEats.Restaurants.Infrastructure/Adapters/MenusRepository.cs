using Microsoft.EntityFrameworkCore;
using MUEats.Restaurants.Application.DTOs;
using MUEats.Restaurants.Application.Ports;
using MUEats.Restaurants.Core.Domain.Menu;
using MUEats.Restaurants.Core.Domain.Menu.Entities;
using MUEats.Restaurants.Infrastructure.Persistence.Contexts;

namespace MUEats.Restaurants.Infrastructure.Adapters;

public class MenusRepository : IMenusRepository
{
    private readonly RestaurantsDbContext _context;

    public MenusRepository(RestaurantsDbContext context)
    {
        _context = context;
    }

    public Task<Menu?> GetByIdAsync(Guid id, CancellationToken ct)
    {
        return _context.Menus.
            AsSplitQuery().
            Include(x => x.Categories).
            Include(x => x.MenuItems).
            FirstOrDefaultAsync(x => x.Id == id, ct);
    }

    public Task<MenuDto?> GetDtoByIdAsync(Guid restaurantId, CancellationToken ct)
    {
        return _context.Menus
            .AsSplitQuery()
            .Where(x => x.RestaurantId == restaurantId)
            .Select(x => new MenuDto
            {
                Id = x.Id,
                RestaurantId = restaurantId,
                MenuCategories = x.Categories.Select(c => new MenuCategoryDto
                {
                    Id = c.Id,
                    Name = c.Name,
                    MenuItems = x.MenuItems
                        .Where(y => y.CategoryId == c.Id && y.IsAvailable)
                        .Select(i => new MenuItemDto
                    {
                        Id = i.Id,
                        Name = i.Name,
                        Price = i.Price,
                    }).ToList()
                }).ToList()
            }).FirstOrDefaultAsync(ct);
    }

    public Task AddAsync(Menu menu, CancellationToken ct)
    {
        return _context.AddAsync(menu, ct)
            .AsTask();
    }

    public Task UpdateAsync(Menu menu, CancellationToken ct)
    {
        _context.Update(menu);
        return Task.CompletedTask;
    }

    public Task DeleteAsync(Menu menu, CancellationToken ct)
    {
        _context.Remove(menu);
        return Task.CompletedTask;
    }

    public Task<bool> AnyAsync(Guid restaurantId, CancellationToken ct)
    {
        return  _context.Menus.AnyAsync(x => x.RestaurantId == restaurantId, ct);
    }
}