using Microsoft.EntityFrameworkCore;
using MUEats.Application.Ports;
using MUEats.Core.Domain.Restaurant;
using MUEats.Infrastructure.Persistence;

namespace MUEats.Infrastructure.Adapters.Repositories;

public class RestaurantsRepository(MueDbContext context) : IRestaurantsRepository
{
    public Task AddAsync(Restaurant restaurant, CancellationToken ct)
    {
        return context.Restaurants.AddAsync(restaurant, ct)
            .AsTask();
    }

    public Task<Restaurant?> GetByIdAsync(Guid id, CancellationToken ct)
    {
        return context.Restaurants.Where(x => x.Id == id)
            .Include(x => x.FoodItems)
            .FirstOrDefaultAsync(ct);
    }

    public Task DeleteAsync(Restaurant restaurant, CancellationToken ct)
    {
        context.Restaurants.Remove(restaurant);
        
        return Task.CompletedTask;
    }

    public Task UpdateAsync(Restaurant restaurant, CancellationToken ct)
    {
        context.Restaurants.Update(restaurant);
        return Task.CompletedTask;
    }
}