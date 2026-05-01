using System.Collections.ObjectModel;
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
        var categoryCheck = _categories.Any(x => x.Name == categoryName && x.Description == categoryDescription);

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
}