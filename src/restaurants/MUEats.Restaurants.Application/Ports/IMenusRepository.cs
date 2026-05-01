using MUEats.Restaurants.Core.Domain.Menu;

namespace MUEats.Restaurants.Application.Ports;

public interface IMenusRepository
{
    Task<Menu?> GetByIdAsync(Guid id, CancellationToken ct);
    
    Task AddAsync(Menu restaurant, CancellationToken ct);
    
    Task UpdateAsync(Menu restaurant, CancellationToken ct);
    
    Task DeleteAsync(Menu restaurant, CancellationToken ct);
    
    Task<bool> AnyAsync(Guid id, CancellationToken ct);
}