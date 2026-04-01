using MUEats.Application.Dto.Restaurant;
using MUEats.Application.Ports;
using MUEats.Core.Domain.Events.Order;
using MUEats.Core.Domain.Restaurant;
using MUEats.Core.Domain.Restaurant.Entities;

namespace MUEats.Application.Services;


public class RestaurantsService(
    IRestaurantsRepository restaurantsRepository, 
    IUnitOfWork uow,
    IFoodItemsRepository foodItemsRepository)
{
    public async Task CreateAsync(CreateRestaurantDto dto, CancellationToken ct)
    {
        await uow.BeginTransactionAsync(ct);

        //todo Validation
        
        if (dto.OpeningHours == TimeSpan.Zero || dto.ClosingHours == TimeSpan.Zero)
        {
            throw new Exception("Opening hours and closing hours are required");
        }

        if (dto.OpeningHours > dto.ClosingHours)
        {
            throw new Exception("Opening hours can not be greater than closing hours");
        }
        
        var restaurant = new Restaurant
        {
            Id = Guid.NewGuid(),
            Name = dto.Name,
            Address = dto.Address,
            OpeningHours = dto.OpeningHours,
            ClosingHours = dto.ClosingHours,
        };
        
        await restaurantsRepository.AddAsync(restaurant, ct);
        
        await uow.SaveChangesAsync(ct);
        await uow.CommitTransactionAsync(ct);
    }

    public async Task AddFoodItemAsync(Guid userId, CreateFoodItemDto dto, CancellationToken ct)
    {
        await uow.BeginTransactionAsync(ct);
        
        var managerId = await restaurantsRepository.GetManagerIdAsync(dto.RestaurantId, ct);

        if (managerId != userId)
        {
            throw new UnauthorizedAccessException("Forbidden");
        }

        if (dto.Name.Length <= 2 || dto.Name.Length > 64)
        {
            throw new ArgumentException("Name must be between 2 and 64 characters long");
        }

        if (dto.Price <= 0)
        {
            throw new ArgumentException("Price must be greater than 0");
        }
        
        var foodItem = new FoodItem
        {
            Id = Guid.NewGuid(),
            Name = dto.Name,
            Description = dto.Description,
            RestaurantId = dto.RestaurantId,
            Price = dto.Price,
            IsAvailable = false,
        };
        
        await foodItemsRepository.AddAsync(foodItem, ct);
        
        await uow.SaveChangesAsync(ct);
        await uow.CommitTransactionAsync(ct);
    }

    public async Task<RestaurantDto> GetByIdAsync(Guid id, CancellationToken ct)
    {
        var restaurant = await restaurantsRepository.GetDtoByIdAsync(id, ct);

        if (restaurant is null)
        {
            throw new ArgumentException("Restaurant not found");
        }
        
        return restaurant;
    }

    public async Task<List<RestaurantDto>> GetAllAsync(GetRestaurantsQuery query, CancellationToken ct)
    {
        if (query.PageSize <= 0 || query.Page <= 0)
        {
            throw new ArgumentException("PageSize and Page cannot be less than 0");
        }
        
        var restaurants = await restaurantsRepository.GetAllAsync(query.Page, query.PageSize, ct);
        
        return restaurants;
    }
}