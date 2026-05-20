using MUEats.Restaurants.Core.Domain;
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

    [Fact]
    public void Test_AddCategory_EmptyName_ReturnsError()
    {
        var menu = CreateMenu();
        
        var categoryName = "";
        var categoryDescription = "Check out our drinks";
        
        var categoryResult = menu.AddCategory(categoryName, categoryDescription);
        
        Assert.False(categoryResult.IsSuccess);
        Assert.DoesNotContain(menu.Categories, c => c.Name == categoryName);
        Assert.Equal(categoryResult.Error, DomainErrors.MenuCategory.CategoryNameIsEmpty);
    }

    [Fact]
    public void Test_AddCategory_AlreadyExists_ReturnsError()
    {
        var menu = CreateMenu();

        var categoryName = "Drinks";
        var categoryDescription = "Check out our drinks";

        menu.AddCategory(categoryName, categoryDescription);
        
        var categoryResult = menu.AddCategory(categoryName, categoryDescription);
        
        Assert.False(categoryResult.IsSuccess);
        Assert.Single(menu.Categories);
        Assert.Equal(categoryResult.Error, DomainErrors.Menu.CategoryAlreadyExists);
    }

    [Fact]
    public void Test_AddMenuItem_ValidArguments_ReturnsSuccessAndCreatesMenuItem()
    {
        var menu = CreateMenu();

        menu.AddCategory("Drinks", "Check out our drinks");
        
        var categoryId = menu.Categories.FirstOrDefault(c => c.Name == "Drinks")!.Id;

        var itemName = "BoboTea";
        var itemDescription = "Bobo milk tea";
        var itemPrice = 100.0m;
        
        var result = menu.AddMenuItem(itemName, itemDescription, itemPrice, categoryId);
        
        Assert.True(result.IsSuccess);
        Assert.Contains(menu.MenuItems, c => c.Name == itemName);
    }

    [Fact]
    public void Test_AddMenuItem_CategoryDoesNotExist_ReturnsError()
    {
        var menu = CreateMenu();
        
        var itemName = "BoboTea";
        var itemDescription = "Bobo milk tea";
        var itemPrice = 100.0m;
        
        var result = menu.AddMenuItem(itemName, itemDescription, itemPrice, Guid.Empty);
        
        Assert.False(result.IsSuccess);
        Assert.DoesNotContain(menu.MenuItems, c => c.Name == itemName);
        Assert.Equal(result.Error, DomainErrors.Menu.CategoryDoesNotExists);
    }

    [Fact]
    public void Test_AddMenuItem_NameIsEmpty_ReturnsError()
    {
        var menu = CreateMenu();
        
        menu.AddCategory("Drinks", "Check out our drinks");
        
        var categoryId = menu.Categories.FirstOrDefault(c => c.Name == "Drinks")!.Id;

        var itemName = "";
        var itemDescription = "Bobo milk tea";
        var itemPrice = 100.0m;
        
        var result = menu.AddMenuItem(itemName, itemDescription, itemPrice, categoryId);
        
        Assert.False(result.IsSuccess);
        Assert.DoesNotContain(menu.MenuItems, c => c.Name == itemName);
        Assert.Equal(result.Error, DomainErrors.MenuItem.ItemNameIsEmpty);
    }

    [Fact]
    public void Test_AddMenuItem_ItemPriceLessThanZero_ReturnsError()
    {
        var menu = CreateMenu();
        
        menu.AddCategory("Drinks", "Check out our drinks");
        
        var categoryId = menu.Categories.FirstOrDefault(c => c.Name == "Drinks")!.Id;

        var itemName = "Bobo tea";
        var itemDescription = "Bobo milk tea";
        var itemPrice = -10000m;
        
        var result = menu.AddMenuItem(itemName, itemDescription, itemPrice, categoryId);
        
        Assert.False(result.IsSuccess);
        Assert.DoesNotContain(menu.MenuItems, c => c.Name == itemName);
        Assert.Equal(result.Error, DomainErrors.MenuItem.ItemPriceLessThanZero);
    }
    
    [Fact]
    public void Test_AddMenuItem_ItemAlreadyExists_ReturnsError()
    {
        var menu = CreateMenu();

        menu.AddCategory("Drinks", "Check out our drinks");
        
        var categoryId = menu.Categories.FirstOrDefault(c => c.Name == "Drinks")!.Id;

        var itemName = "BoboTea";
        var itemDescription = "Bobo milk tea";
        var itemPrice = 100.0m;
        
        menu.AddMenuItem(itemName, itemDescription, itemPrice, categoryId);

        var secondItemName = "BoboTea";
        var secondItemDescription = "Bobo milk tea";
        var secondItemPrice = 100.0m;

        var result = menu.AddMenuItem(secondItemName, secondItemDescription, secondItemPrice, categoryId);
        
        Assert.False(result.IsSuccess);
        Assert.Single(menu.MenuItems);
        Assert.Equal(result.Error, DomainErrors.Menu.MenuItemAlreadyExists);
    }

    [Fact]
    public void Test_AddOptionsGroup_ValidArguments_ReturnsSuccess()
    {       
        var menu = CreateMenu();
        
        menu.AddCategory("Drinks", "Check out our drinks");
        
        var categoryId = menu.Categories.FirstOrDefault(c => c.Name == "Drinks")!.Id;

        var itemName = "Bobo tea";
        var itemDescription = "Bobo milk tea";
        var itemPrice = 100m;
        
        menu.AddMenuItem(itemName, itemDescription, itemPrice, categoryId);
        
        var itemId = menu.MenuItems.FirstOrDefault(m => m.Name == itemName)!.Id;
        
        var groupName = "Drink temperature";
        var groupDescription = "Please let us know your preferable drink temperature";
        
        var result = menu.AddOptionsGroup(itemId, groupName, groupDescription);

        var item = menu.MenuItems.FirstOrDefault(x => x.Id == itemId);
        var optionsGroupExists = item!.OptionsGroups.Any(g => g.Name == groupName);
        
        Assert.True(result.IsSuccess);
        Assert.True(optionsGroupExists);
    }   
    
    [Fact]
    public void Test_AddOptionsGroup_ItemDoesNotExists_ReturnsError()
    {       
        var menu = CreateMenu();
        
        var groupName = "Drink temperature";
        var groupDescription = "Please let us know your preferable drink temperature";
        
        var result = menu.AddOptionsGroup(Guid.Empty, groupName, groupDescription);
        
        Assert.False(result.IsSuccess);
        Assert.Equal(result.Error, DomainErrors.Menu.MenuItemDoesNotExist);
    }

    [Fact]
    public void Test_AddOptionsGroup_GroupNameIsEmpty_ReturnsError()
    {
        var menu = CreateMenu();
        
        menu.AddCategory("Drinks", "Check out our drinks");
        
        var categoryId = menu.Categories.FirstOrDefault(c => c.Name == "Drinks")!.Id;

        var itemName = "Bobo tea";
        var itemDescription = "Bobo milk tea";
        var itemPrice = 100m;
        
        menu.AddMenuItem(itemName, itemDescription, itemPrice, categoryId);
        
        var itemId = menu.MenuItems.FirstOrDefault(m => m.Name == itemName)!.Id;
        
        var groupName = "";
        var groupDescription = "Please let us know your preferable drink temperature";
        
        var result = menu.AddOptionsGroup(itemId, groupName, groupDescription);

        var item = menu.MenuItems.FirstOrDefault(x => x.Id == itemId);
        var optionsGroupExists = item!.OptionsGroups.Any(g => g.Name == groupName);
        
        Assert.False(result.IsSuccess);
        Assert.False(optionsGroupExists);
        Assert.Equal(result.Error, DomainErrors.MenuOptionsGroup.OptionsGroupNameIsEmpty);
    }

    [Fact]
    public void Test_AddOptionsGroup_GroupAlreadyExists_ReturnsError()
    {
        var menu = CreateMenu();
        
        menu.AddCategory("Drinks", "Check out our drinks");
        
        var categoryId = menu.Categories.FirstOrDefault(c => c.Name == "Drinks")!.Id;

        var itemName = "Bobo tea";
        var itemDescription = "Bobo milk tea";
        var itemPrice = 100m;
        
        menu.AddMenuItem(itemName, itemDescription, itemPrice, categoryId);
        
        var itemId = menu.MenuItems.FirstOrDefault(m => m.Name == itemName)!.Id;
        
        var groupName = "Drink temperature";
        var groupDescription = "Please let us know your preferable drink temperature";
        
        menu.AddOptionsGroup(itemId, groupName, groupDescription);
        
        var secondGroupName = "Drink temperature";
        var secondGroupDescription = "Please let us know your preferable drink temperature";
        
        var result = menu.AddOptionsGroup(itemId, secondGroupName, secondGroupDescription);

        var item = menu.MenuItems.FirstOrDefault(x => x.Id == itemId);
        var optionsGroupCount = item!.OptionsGroups.Count;
        
        Assert.False(result.IsSuccess);
        Assert.Equal(1, optionsGroupCount);
        Assert.Equal(result.Error, DomainErrors.MenuItem.ItemOptionsGroupAlreadyExists);
    }

    [Fact]
    public void Test_AddItemOption_ValidArguments_ReturnsSuccess()
    {
        var menu = CreateMenu();
        
        menu.AddCategory("Drinks", "Check out our drinks");
        
        var categoryId = menu.Categories.FirstOrDefault(c => c.Name == "Drinks")!.Id;

        var itemName = "Bobo tea";
        var itemDescription = "Bobo milk tea";
        var itemPrice = 100m;
        
        menu.AddMenuItem(itemName, itemDescription, itemPrice, categoryId);
        
        var itemId = menu.MenuItems.FirstOrDefault(m => m.Name == itemName)!.Id;
        
        var groupName = "Drink temperature";
        var groupDescription = "Please let us know your preferable drink temperature";
        
        var item = menu.MenuItems.FirstOrDefault(x => x.Id == itemId);
        
        menu.AddOptionsGroup(itemId, groupName, groupDescription);
        
        var group = item.OptionsGroups.FirstOrDefault(g => g.Name == groupName);

        var firstItemOptionResult = menu.AddItemOption(group.Id, "hot", null);
        var secondItemOptionResult = menu.AddItemOption(group.Id, "cold", null);
        
        Assert.True(firstItemOptionResult.IsSuccess);
        Assert.True(secondItemOptionResult.IsSuccess);
        Assert.Equal(2, group.ItemOptions.Count);
    }

    [Fact]
    public void Test_AddItemOption_ItemOptionAlreadyExists_ReturnsError()
    {
        var menu = CreateMenu();
        
        menu.AddCategory("Drinks", "Check out our drinks");
        
        var categoryId = menu.Categories.FirstOrDefault(c => c.Name == "Drinks")!.Id;

        var itemName = "Bobo tea";
        var itemDescription = "Bobo milk tea";
        var itemPrice = 100m;
        
        menu.AddMenuItem(itemName, itemDescription, itemPrice, categoryId);
        
        var itemId = menu.MenuItems.FirstOrDefault(m => m.Name == itemName)!.Id;
        
        var groupName = "Drink temperature";
        var groupDescription = "Please let us know your preferable drink temperature";
        
        var item = menu.MenuItems.FirstOrDefault(x => x.Id == itemId);
        
        menu.AddOptionsGroup(itemId, groupName, groupDescription);
        
        var group = item.OptionsGroups.FirstOrDefault(g => g.Name == groupName);

        menu.AddItemOption(group.Id, "hot", null);
        var result = menu.AddItemOption(group.Id, "hot", null);
        
        Assert.False(result.IsSuccess);
        Assert.Single(group.ItemOptions);
        Assert.Equal(result.Error, DomainErrors.MenuOptionsGroup.OptionAlreadyExists);
    }

    [Fact]
    public void Test_AddItemOption_OptionValueIsEmpty_ReturnsError()
    {
        var menu = CreateMenu();
        
        menu.AddCategory("Drinks", "Check out our drinks");
        
        var categoryId = menu.Categories.FirstOrDefault(c => c.Name == "Drinks")!.Id;

        var itemName = "Bobo tea";
        var itemDescription = "Bobo milk tea";
        var itemPrice = 100m;
        
        menu.AddMenuItem(itemName, itemDescription, itemPrice, categoryId);
        
        var itemId = menu.MenuItems.FirstOrDefault(m => m.Name == itemName)!.Id;
        
        var groupName = "Drink temperature";
        var groupDescription = "Please let us know your preferable drink temperature";
        
        var item = menu.MenuItems.FirstOrDefault(x => x.Id == itemId);
        
        menu.AddOptionsGroup(itemId, groupName, groupDescription);
        
        var group = item.OptionsGroups.FirstOrDefault(g => g.Name == groupName);

        var result = menu.AddItemOption(group.Id, "", null); 
        
        Assert.False(result.IsSuccess);
        Assert.Equal(result.Error, DomainErrors.MenuItemOption.OptionValueIsEmpty);
    }
    
    private static Menu CreateMenu()
    {
        var restaurantId = Guid.NewGuid();

        return Menu.Create(restaurantId).Value;
    }
}