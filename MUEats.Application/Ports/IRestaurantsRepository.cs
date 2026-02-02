using MUEats.Core.Domain.Restaurant;

namespace MUEats.Application.Ports;

public interface IRestaurantsRepository
{
    Task AddAsync(Restaurant restaurant, CancellationToken ct);
    Task<Restaurant?> GetByIdAsync(Guid id, CancellationToken ct);
    Task DeleteAsync(Restaurant restaurant, CancellationToken ct);
    Task UpdateAsync(Restaurant restaurant, CancellationToken ct);
}