using MUEats.Restaurants.Core.Domain.Restaurant;

namespace MUEats.Restaurants.Application.Ports;

public interface IRestaurantsRepository
{
    Task<Restaurant?> GetByIdAsync(Guid id, CancellationToken ct);
    
    Task AddAsync(Restaurant restaurant, CancellationToken ct);
    
    Task UpdateAsync(Restaurant restaurant, CancellationToken ct);
    
    Task DeleteAsync(Restaurant restaurant, CancellationToken ct);
    
    Task<bool> AnyAsync(Guid id, CancellationToken ct);
    
    Task<bool> AnyAsync(string name, CancellationToken ct);
}