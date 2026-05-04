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
    
    public IReadOnlyCollection<ItemOption> ItemOptions => _itemOptions.AsReadOnly();

    public static Result<OptionsGroup, Error> Create(string name, 
        string? description,  
        Guid menuItemId)
    {
        if (string.IsNullOrWhiteSpace(name))
        {
            return DomainErrors.MenuOptionsGroup.OptionsGroupNameIsEmpty;
        }
        
        return new OptionsGroup(name, description,  menuItemId);
    }

    public UnitResult<Error> AddItemOption(string optionValue, decimal? additionalPrice)
    {
        var check = _itemOptions.Any(x => x.Value == optionValue);

        if (check)
        {
            return DomainErrors.MenuOptionsGroup.OptionAlreadyExists;
        }
        
        var itemOption = ItemOption.Create(optionValue, Id, additionalPrice);

        if (itemOption.IsFailure)
        {
            return itemOption.Error;
        }
        
        _itemOptions.Add(itemOption.Value);
        
        return UnitResult.Success<Error>();
    }

    public UnitResult<Error> DeleteItemOption(Guid optionId)
    {
        var option = _itemOptions.FirstOrDefault(x => x.Id == optionId);

        if (option is null)
        {
            return DomainErrors.MenuOptionsGroup.ItemOptionDoesNotExist;
        }

        _itemOptions.Remove(option);

        return UnitResult.Success<Error>();
    }
}