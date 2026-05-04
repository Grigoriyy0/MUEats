using CSharpFunctionalExtensions;
using MUEats.Restaurants.Core.Domain.Menu.Entities;
using Primitives;

namespace MUEats.Restaurants.Core.Domain.Menu;

public class Menu
{
    private Menu(Guid restaurantId)
    {
        Id = Guid.NewGuid();
        RestaurantId = restaurantId;
    }
    
    public Guid Id { get; init; }
    
    public Guid RestaurantId { get; init; }

    private readonly List<Category> _categories = [];
    
    public IReadOnlyCollection<Category> Categories => _categories.AsReadOnly();
    
    private readonly List<MenuItem> _menuItems = [];
    
    public IReadOnlyCollection<MenuItem> MenuItems => _menuItems.AsReadOnly();

    public bool IsActive { get; private set; } = false;

    public static Result<Menu, Error> Create(Guid restaurantId)
    {
        return new Menu(restaurantId);
    }

    public UnitResult<Error> AddCategory(string categoryName, string? categoryDescription)
    {
        var categoryCheck = _categories.Any(x => x.Name == categoryName);

        if (categoryCheck)
        {
            return DomainErrors.Menu.CategoryAlreadyExists;
        }

        var categoryResult = Category.Create(categoryName, categoryDescription, Id);

        if (categoryResult.IsFailure)
        {
            return categoryResult.Error;
        }
        
        _categories.Add(categoryResult.Value);
        
        return UnitResult.Success<Error>();
    }

    public UnitResult<Error> AddMenuItem(string itemName, 
        string? itemDescription, 
        decimal itemPrice,
        Guid categoryId)
    {
        var check = _menuItems.Any(x => x.Name == itemName);

        if (check)
        {
            return DomainErrors.Menu.MenuItemAlreadyExists;
        }

        var categoryCheck = _categories.Any(x => x.Id == categoryId);

        if (!categoryCheck)
        {
            return DomainErrors.Menu.CategoryDoesNotExists;
        }

        var itemResult = MenuItem.Create(itemName, itemPrice, itemDescription,  categoryId, Id);

        if (itemResult.IsFailure)
        {
            return itemResult.Error;
        }
        
        _menuItems.Add(itemResult.Value);
        
        return UnitResult.Success<Error>();
    }

    public UnitResult<Error> UpdateMenuItem(Guid itemId,
        string itemName,
        string? itemDescription,
        decimal itemPrice,
        bool isAvailable,
        Guid categoryId)
    {
        var categoryCheck = _categories.Any(x => x.Id == categoryId);

        if (!categoryCheck)
        {
            return DomainErrors.Menu.CategoryDoesNotExists;
        }
        
        var item = _menuItems.FirstOrDefault(x => x.Id == itemId);

        if (item is null)
        {
            return DomainErrors.Menu.MenuItemDoesNotExist;
        }

        var result = item.Update(itemName, itemPrice, itemDescription, isAvailable, categoryId);

        if (result.IsFailure)
        {
            return result.Error;
        }
        
        return UnitResult.Success<Error>();
    }

    public UnitResult<Error> AddOptionsGroup(Guid itemId, 
        string groupName, 
        string? groupDescription)
    {
        var item = _menuItems.FirstOrDefault(x => x.Id == itemId);

        if (item is null)
        {
            return DomainErrors.Menu.MenuItemDoesNotExist;
        }

        var result = item.AddOptionsGroup(groupName, groupDescription);

        if (result.IsFailure)
        {
            return result.Error;
        }
        
        return UnitResult.Success<Error>();
    }

    public UnitResult<Error> DeleteOptionsGroup(Guid groupId)
    {
        var item = _menuItems.FirstOrDefault(x => x.OptionsGroups.Any(g => g.Id == groupId));

        if (item is null)
        {
            return DomainErrors.Menu.MenuItemDoesNotExist;
        }

        var result = item.DeleteOptionsGroup(groupId);

        if (result.IsFailure)
        {
            return result.Error;
        }
        
        return UnitResult.Success<Error>();
    }
    

    public UnitResult<Error> AddItemOption(Guid groupId,
        string optionValue,
        decimal? additionalPrice)
    {
        var group = MenuItems.SelectMany(x => x.OptionsGroups)
            .FirstOrDefault(g => g.Id == groupId);
        
        if (group is null)
        {
            return DomainErrors.MenuItem.ItemOptionsGroupDoesNotExists;
        }

        var result = group.AddItemOption(optionValue, additionalPrice);

        if (result.IsFailure)
        {
            return result.Error;
        }

        return UnitResult.Success<Error>();
    }

    public UnitResult<Error> DeleteItemOption(Guid optionId)
    {
        var group = MenuItems.SelectMany(i => i.OptionsGroups)
            .FirstOrDefault(x => x.ItemOptions.Any(o => o.Id == optionId));

        if (group is null)
        {
            return DomainErrors.MenuOptionsGroup.ItemOptionDoesNotExist;
        }

        var result = group.DeleteItemOption(optionId);

        if (result.IsFailure)
        {
            return result.Error;
        }

        return UnitResult.Success<Error>();
    }
}