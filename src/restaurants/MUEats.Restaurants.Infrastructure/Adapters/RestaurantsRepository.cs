using Microsoft.EntityFrameworkCore;
using MUEats.Restaurants.Application.Ports;
using MUEats.Restaurants.Core.Domain.Restaurant;
using MUEats.Restaurants.Infrastructure.Persistence.Contexts;

namespace MUEats.Restaurants.Infrastructure.Adapters;

public sealed class RestaurantsRepository : IRestaurantsRepository
{
    private readonly RestaurantsDbContext _context;

    public RestaurantsRepository(RestaurantsDbContext context)
    {
        _context = context;
    }

    public Task<Restaurant?> GetByIdAsync(Guid id, CancellationToken ct)
    {
        return _context.Restaurants.FirstOrDefaultAsync(x => x.Id == id, ct);
    }

    public Task AddAsync(Restaurant restaurant, CancellationToken ct)
    {
        return _context.AddAsync(restaurant, ct)
            .AsTask();
    }

    public Task UpdateAsync(Restaurant restaurant, CancellationToken ct)
    {
        _context.Update(restaurant);
        return Task.CompletedTask;
    }

    public Task DeleteAsync(Restaurant restaurant, CancellationToken ct)
    {
        _context.Remove(restaurant);
        return Task.CompletedTask;
    }

    public Task<bool> AnyAsync(Guid id, CancellationToken ct)
    {
        return  _context.Restaurants.AnyAsync(x => x.Id == id, ct);
    }

    public Task<bool> AnyAsync(string name, CancellationToken ct)
    {
        return  _context.Restaurants.AnyAsync(x => x.Name == name, ct);
    }
}