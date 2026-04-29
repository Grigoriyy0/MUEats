using System.Collections.ObjectModel;
using CSharpFunctionalExtensions;
using Primitives;

namespace MUEats.Restaurants.Core.Domain.Menu.Entities;

public class MenuItem
{
    private MenuItem(string name,
        decimal price,
        string? description,
        Guid? categoryId)
    {
        Id = Guid.NewGuid();
        Name = name;
        Price = price;
        Description = description;
        CategoryId = categoryId;
    }
    
    public Guid Id { get; init; }
    
    public string Name { get; private set; }
    
    public decimal Price { get; private set; }
    
    public string? Description { get; private set; }
    
    public bool IsAvailable { get; private set; }
    
    private readonly List<OptionsGroup>  _optionsGroups = [];
    
    public ReadOnlyCollection<OptionsGroup> OptionsGroups => _optionsGroups.AsReadOnly();
    
    public Guid? CategoryId { get; private set; }
    
    public static Result<MenuItem, Error> Create(string name, 
        decimal price, 
        string? description,
        Guid? categoryId)
    {
        if (string.IsNullOrEmpty(name))
        {
            return DomainErrors.MenuItem.ItemNameIsEmpty;
        }

        if (price < 0)
        {
            return DomainErrors.MenuItem.ItemPriceLessThanZero;
        }
        
        return new MenuItem(name, price, description,  categoryId);
    }

    public UnitResult<Error> AddOptionsGroup(string groupName, string? description)
    {
        var optionsGroupResult = OptionsGroup.Create(groupName, description);

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
}