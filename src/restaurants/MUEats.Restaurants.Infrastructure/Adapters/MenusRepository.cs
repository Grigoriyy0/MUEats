using Microsoft.EntityFrameworkCore;
using MUEats.Restaurants.Application.Ports;
using MUEats.Restaurants.Core.Domain.Menu;
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
        return _context.Menus.FirstOrDefaultAsync(x => x.Id == id, ct);
    }

    public Task AddAsync(Menu menu, CancellationToken ct)
    {
        return _context.AddAsync(menu, ct)
            .AsTask();
    }

    public Task UpdateAsync(Menu restaurant, CancellationToken ct)
    {
        _context.Update(restaurant);
        return Task.CompletedTask;
    }

    public Task DeleteAsync(Menu restaurant, CancellationToken ct)
    {
        _context.Remove(restaurant);
        return Task.CompletedTask;
    }

    public Task<bool> AnyAsync(Guid id, CancellationToken ct)
    {
        return  _context.Restaurants.AnyAsync(x => x.Id == id, ct);
    }
}