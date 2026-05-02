using CSharpFunctionalExtensions;
using Primitives;

namespace MUEats.Restaurants.Core.Domain.Menu.Entities;

public class MenuItem
{
    private MenuItem(string name,
        decimal price,
        string? description,
        Guid? categoryId,
        Guid menuId)
    {
        Id = Guid.NewGuid();
        Name = name;
        Price = price;
        Description = description;
        CategoryId = categoryId;
        MenuId = menuId;
    }
    
    public Guid Id { get; init; }
    
    public Guid MenuId { get; private set; }
    
    public string Name { get; private set; }
    
    public decimal Price { get; private set; }
    
    public string? Description { get; private set; }
    
    public bool IsAvailable { get; private set; }
    
    private readonly List<OptionsGroup>  _optionsGroups = [];
    
    public IReadOnlyCollection<OptionsGroup> OptionsGroups => _optionsGroups.AsReadOnly();
    
    public Guid? CategoryId { get; private set; }
    
    public static Result<MenuItem, Error> Create(string name, 
        decimal price, 
        string? description,
        Guid? categoryId,
        Guid menuId)
    {
        if (string.IsNullOrEmpty(name))
        {
            return DomainErrors.MenuItem.ItemNameIsEmpty;
        }

        if (price < 0)
        {
            return DomainErrors.MenuItem.ItemPriceLessThanZero;
        }
        
        return new MenuItem(name, price, description,  categoryId, menuId);
    }

    public UnitResult<Error> AddOptionsGroup(string groupName, string? description)
    {
        var optionsGroupExists = _optionsGroups.Any(x => x.Name == groupName);

        if (optionsGroupExists)
        {
            return DomainErrors.MenuItem.ItemOptionsGroupAlreadyExists;   
        }
        
        var optionsGroupResult = OptionsGroup.Create(groupName, description, Id);

        if (optionsGroupResult.IsFailure)
        {
            return optionsGroupResult.Error;
        }
        
        _optionsGroups.Add(optionsGroupResult.Value);
        
        return UnitResult.Success<Error>();
    }

    public UnitResult<Error> AddItemOption(Guid optionsGroupId, string value)
    {
        var optionsGroup = _optionsGroups.FirstOrDefault(x => x.Id == optionsGroupId);

        if (optionsGroup is null)
        {
            return DomainErrors.MenuItem.ItemOptionsGroupIsNotFound;
        }
        
        var itemResult = optionsGroup.AddItemOption(value);

        if (itemResult.IsFailure)
        {
            return itemResult.Error;
        }
        
        return  UnitResult.Success<Error>();
    }

    public UnitResult<Error> Update(string itemName,
        decimal price,
        string? description,
        bool isAvailable,
        Guid categoryId)
    {
        if (string.IsNullOrEmpty(itemName))
        {
            return DomainErrors.MenuItem.ItemNameIsEmpty;
        }

        Name = itemName;
        Description = description;
        IsAvailable = isAvailable;
        CategoryId = categoryId;
        
        return UnitResult.Success<Error>();
    }
}