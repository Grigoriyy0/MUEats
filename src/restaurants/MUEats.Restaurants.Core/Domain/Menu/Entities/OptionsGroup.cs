using System.Collections.ObjectModel;
using CSharpFunctionalExtensions;
using MUEats.Restaurants.Core.Domain.Menu.Entities.ValueObjects;
using Primitives;

namespace MUEats.Restaurants.Core.Domain.Menu.Entities;

public class OptionsGroup
{
    private OptionsGroup(string name, 
        string? description,
        Guid menuItemId)
    {
        Id = Guid.NewGuid();
        Name = name;
        Description = description;
        MenuItemId = menuItemId;
    }
    
    public Guid Id { get; init; }
    
    public Guid MenuItemId { get; private set; }
    
    public string Name { get; private set; }
    
    public string? Description { get; private set; }
    
    private readonly List<ItemOption> _itemOptions = [];
    
    public ReadOnlyCollection<ItemOption> ItemOptions => _itemOptions.AsReadOnly();

    public static Result<OptionsGroup, Error> Create(string name, 
        string? description,  
        Guid menuItemId)
    {
        return new OptionsGroup(name, description,  menuItemId);
    }

    public UnitResult<Error> AddItemOption(string optionValue)
    {
        var check = _itemOptions.Any(x => x.Value == optionValue);

        if (check)
        {
            return DomainErrors.MenuOptionsGroup.OptionAlreadyExists;
        }
        
        var itemOption = ItemOption.Create(optionValue, Id);

        if (itemOption.IsFailure)
        {
            return itemOption.Error;
        }
        
        _itemOptions.Add(itemOption.Value);
        
        return UnitResult.Success<Error>();
    }
}