using MUEats.Restaurants.Core.Domain.Menu;

namespace MUEats.Restaurants.Domain.UnitTests.MenuTests;

public class MenuAggregateTests
{
    [Fact]
    public void Test_Create_ValidArguments_ReturnsSuccess()
    {
        var restaurantId = Guid.NewGuid();

        var menuResult = Menu.Create(restaurantId);
        
        Assert.True(menuResult.IsSuccess);
        Assert.Equal(menuResult.Value.RestaurantId, restaurantId);
    }
    
    [Fact]
    public void Test_AddCategory_ValidArguments_ReturnsSuccessAndCreatesCategory()
    {
        var menu = CreateMenu();

        var categoryName = "Drinks";
        var categoryDescription = "Check out our drinks";
        
        var categoryResult = menu.AddCategory(categoryName, categoryDescription);
        
        Assert.True(categoryResult.IsSuccess);
        Assert.Contains(menu.Categories, c => c.Name == categoryName);
    }

    private static Menu CreateMenu()
    {
        var restaurantId = Guid.NewGuid();

        return Menu.Create(restaurantId).Value;
    }
}