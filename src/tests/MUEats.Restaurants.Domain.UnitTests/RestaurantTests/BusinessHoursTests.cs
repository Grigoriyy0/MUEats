using MUEats.Restaurants.Core.Domain;
using MUEats.Restaurants.Core.Domain.Restaurant.ValueObjects;

namespace MUEats.Restaurants.Domain.UnitTests.RestaurantTests;

public class BusinessHoursTests
{
    [Fact]
    public void Test_BusinessHoursCreate_EndTimeIsEarlier_ReturnsError()
    {
        var openingTime = new TimeSpan(0, 12, 0, 0);
        var closingTime = new TimeSpan(0, 10, 0, 0);
        
        var businessHours = BusinessHours.Create(openingTime, closingTime);
        
        Assert.False(businessHours.IsSuccess);
        Assert.Equal(businessHours.Error, DomainErrors.RestaurantBusinessHours.EndIsEarlier);
    }
}