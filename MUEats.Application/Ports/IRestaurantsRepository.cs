using MUEats.Application.Dto.Restaurant;
using MUEats.Core.Domain.Restaurant;

namespace MUEats.Application.Ports;

public interface IRestaurantsRepository
{
    Task AddAsync(Restaurant restaurant, CancellationToken ct);
    
    Task<Restaurant?> GetByIdAsync(Guid id, CancellationToken ct);
    
    Task<RestaurantDto?> GetDtoByIdAsync(Guid id, CancellationToken ct);

    Task<Guid> GetManagerIdAsync(Guid restaurantId, CancellationToken ct);
    
    Task DeleteAsync(Restaurant restaurant, CancellationToken ct);
    
    Task UpdateAsync(Restaurant restaurant, CancellationToken ct);

    Task<List<RestaurantDto>> GetAllAsync(int page, int pageSize, CancellationToken ct);
}