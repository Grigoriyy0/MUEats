using MUEats.Core.Domain.Restaurant.Entities;

namespace MUEats.Application.Ports;

public interface IFoodItemsRepository
{
    Task AddAsync(FoodItem foodItem, CancellationToken ct);
    
    Task<FoodItem?> GetByIdAsync(Guid id, CancellationToken ct);
    
    Task DeleteAsync(FoodItem foodItem, CancellationToken ct);
    
    Task UpdateAsync(FoodItem foodItem, CancellationToken ct);
}