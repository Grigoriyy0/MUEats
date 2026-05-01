using CSharpFunctionalExtensions;
using MUEats.Restaurants.Application.DTOs;
using MUEats.Restaurants.Application.Ports;
using MUEats.Restaurants.Core.Domain.Restaurant;
using MUEats.Restaurants.Core.Domain.Restaurant.ValueObjects;
using Primitives;

namespace MUEats.Restaurants.Application.Services;

public class RestaurantsService
{
    private readonly IUnitOfWork _unitOfWork;
    
    private readonly IRestaurantsRepository _restaurantsRepository;

    public RestaurantsService(IUnitOfWork unitOfWork, 
        IRestaurantsRepository restaurantsRepository)
    {
        _unitOfWork = unitOfWork;
        _restaurantsRepository = restaurantsRepository;
    }

    public async Task<UnitResult<Error>> CreateAsync(CreateRestaurantDto dto, CancellationToken ct)
    {
        await _unitOfWork.BeginTransactionAsync(ct);
        
        var restaurantExists = await _restaurantsRepository.AnyAsync(dto.Name, ct);

        if (restaurantExists)
        {
            await _unitOfWork.RollbackTransactionAsync(ct);
            return ApplicationErrors.Restaurant.RestaurantAlreadyExists;
        }
        
        var businessHoursResult = BusinessHours.Create(dto.OpeningTime, dto.ClosingTime);

        if (businessHoursResult.IsFailure)
        {
            await _unitOfWork.RollbackTransactionAsync(ct);
            return businessHoursResult.Error;
        }
        
        var businessHours = businessHoursResult.Value;

        var restaurantResult = Restaurant.Create(dto.Name, dto.Description, businessHours, dto.Address);

        if (restaurantResult.IsFailure)
        {
            await _unitOfWork.RollbackTransactionAsync(ct);
            return restaurantResult.Error;
        }

        await _restaurantsRepository.AddAsync(restaurantResult.Value, ct);
        
        await _unitOfWork.SaveChangesAsync(ct);
        await _unitOfWork.CommitTransactionAsync(ct);
        
        return UnitResult.Success<Error>();
    }
}