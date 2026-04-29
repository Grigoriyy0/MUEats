using CSharpFunctionalExtensions;
using Primitives;

namespace MUEats.Restaurants.Core.Domain.Restaurant.ValueObjects;

public class BusinessHours : ValueObject
{
    private BusinessHours(TimeSpan openingTime, TimeSpan closingTime)
    {
        OpeningTime = openingTime;
        ClosingTime = closingTime;
    }
    
    public static Result<BusinessHours, Error> Create(TimeSpan openingTime, TimeSpan closingTime)
    {
        if (openingTime >= closingTime)
        {
            return DomainErrors.RestaurantBusinessHours.EndIsEarlier;
        }
        
        return new BusinessHours(openingTime, closingTime);
    }
    
    public TimeSpan OpeningTime { get; init; }
    
    public TimeSpan ClosingTime { get; init; }
    
    protected override IEnumerable<object> GetEqualityComponents()
    {
        yield return OpeningTime;
        yield return ClosingTime;
    }
}