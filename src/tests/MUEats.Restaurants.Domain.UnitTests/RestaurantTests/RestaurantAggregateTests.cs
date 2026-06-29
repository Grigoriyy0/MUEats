using MUEats.Restaurants.Core.Domain;
using MUEats.Restaurants.Core.Domain.Restaurant;
using MUEats.Restaurants.Core.Domain.Restaurant.ValueObjects;

namespace MUEats.Restaurants.Domain.UnitTests.RestaurantTests;

public class RestaurantAggregateTests
{
    [Fact]
    public void Test_Create_ValidArguments_ReturnsRestaurant()
    {
        var name = "JCC";
        var description = "Cafe";
        var address = "JCC G/F";
        var latitude = 22.3193;
        var longitude = 114.1694;
        
        var openingTime = new TimeSpan(0, 12, 0, 0);
        var closingTime = new TimeSpan(0, 20, 0, 0);

        var businessHours = BusinessHours.Create(openingTime, closingTime).Value;
        
        var result = Restaurant.Create(name, description, businessHours, address, latitude, longitude);
        
        var restaurant = result.Value;
        
        Assert.True(result.IsSuccess);
        Assert.Equal(restaurant.Name, name);
        Assert.Equal(restaurant.Description, description);
    }

    [Fact]
    public void Test_Create_InvalidName_ReturnsError()
    {
        var name = string.Empty;
        var description = "Cafe";
        var address = "JCC G/F";
        
        var openingTime = new TimeSpan(0, 12, 0, 0);
        var closingTime = new TimeSpan(0, 20, 0, 0);

        var businessHours = BusinessHours.Create(openingTime, closingTime).Value;
        
        var result = Restaurant.Create(name, description, businessHours, address, 22.3193, 114.1694);
        
        Assert.False(result.IsSuccess);
        Assert.Equal(result.Error, DomainErrors.Restaurant.NameIsEmpty);
    }

    [Fact]
    public void Test_Create_InvalidAddress_ReturnsError()
    {
        var name = "JCC";
        var description = "Cafe";
        var address = string.Empty;
        
        var openingTime = new TimeSpan(0, 12, 0, 0);
        var closingTime = new TimeSpan(0, 20, 0, 0);

        var businessHours = BusinessHours.Create(openingTime, closingTime).Value;
        
        var result = Restaurant.Create(name, description, businessHours, address, 22.3193, 114.1694);
        
        Assert.False(result.IsSuccess);
        Assert.Equal(result.Error, DomainErrors.Restaurant.AddressIsEmpty);
    }
    
    [Fact]
    public void Test_UpdateName_EmptyString_ReturnsError()
    {
        var name = string.Empty;
        
        var restaurant = CreateRestaurant();
        
        var result = restaurant.UpdateName(name);
        
        Assert.False(result.IsSuccess);
        Assert.Equal(result.Error, DomainErrors.Restaurant.NameIsEmpty);
    }
    
    [Fact]
    public void UpdateName_ValidName_UpdatesPropertyAndReturnsSuccess()
    {
        var restaurant = CreateRestaurant();
        var newName = "New Name";

        var result = restaurant.UpdateName(newName);

        Assert.True(result.IsSuccess);
        Assert.Equal(newName, restaurant.Name);
    }

    private static Restaurant CreateRestaurant()
    {
        var openingTime = new TimeSpan(0, 12, 0, 0);
        var closingTime = new TimeSpan(0, 20, 0, 0);

        var businessHours = BusinessHours.Create(openingTime, closingTime).Value;
        
        return Restaurant.Create("JCC", null, businessHours, "JCC G/F", 22.3193, 114.1694).Value;
    }
}