using Microsoft.EntityFrameworkCore;
using MUEats.Application.Ports;
using MUEats.Core.Domain.Restaurant.Entities;
using MUEats.Infrastructure.Persistence;

namespace MUEats.Infrastructure.Adapters.Repositories;

public class FoodItemsRepository(MueDbContext context) : IFoodItemsRepository
{
    public Task AddAsync(FoodItem foodItem, CancellationToken ct)
    {
        return context.FoodItems.AddAsync(foodItem, ct)
            .AsTask();
    }

    public Task<FoodItem?> GetByIdAsync(Guid id, CancellationToken ct)
    {
        return context.FoodItems.FirstOrDefaultAsync(x => x.Id == id, ct);
    }

    public Task DeleteAsync(FoodItem foodItem, CancellationToken ct)
    {
        context.FoodItems.Remove(foodItem);
        
        return Task.CompletedTask;
    }

    public Task UpdateAsync(FoodItem foodItem, CancellationToken ct)
    {
        context.FoodItems.Update(foodItem);
        
        return Task.CompletedTask;
    }
}