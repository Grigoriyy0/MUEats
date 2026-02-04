using Microsoft.EntityFrameworkCore;
using MUEats.Application.Dto.Restaurant;
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

    public Task<RestaurantDto?> GetByIdAsync(Guid id, CancellationToken ct)
    {
        return context.Restaurants
            .AsNoTracking()
            .Where(x => x.Id == id)
            .Include(x => x.FoodItems)
            .Select(x => new RestaurantDto
            {
                Id = x.Id,
                Name = x.Name,
                Address = x.Address,
                OpeningHours = x.OpeningHours,
                ClosingHours = x.ClosingHours,
                FoodItems = x.FoodItems
                    .Select(y => new FoodItemDto
                {
                    Id = y.Id,
                    Name = y.Name,
                    Description = y.Description,
                    IsAvailable = y.IsAvailable,
                    Price = y.Price,
                    RestaurantId = y.RestaurantId
                }).ToList()
            }).FirstOrDefaultAsync(ct);
    }

    public Task<List<RestaurantDto>> GetAllAsync(int page, int pageSize, CancellationToken ct)
    {
        return context.Restaurants
            .AsNoTracking()
            .Select(x => new RestaurantDto
            {
                Id = x.Id,
                Name = x.Name,
                Address = x.Address,
                OpeningHours = x.OpeningHours,
                ClosingHours = x.ClosingHours,
                FoodItems = x.FoodItems.Select(y => new FoodItemDto
                {
                    Id = y.Id,
                    Name = y.Name,
                    Description = y.Description,
                    IsAvailable = y.IsAvailable,
                    Price = y.Price
                }).ToList()
            })
            .OrderByDescending(x => x.Id)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(ct);
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