using CSharpFunctionalExtensions;
using Primitives;

namespace MUEats.Restaurants.Core.Domain.Menu.Entities.ValueObjects;

public class ItemOption : ValueObject
{
    private ItemOption(string value, Guid groupId)
    {
        Id = Guid.NewGuid();
        Value = value;
        GroupId = groupId;
    }
    
    public Guid Id { get; private set; }

    public Guid GroupId { get; private set; }
    
    public string Value { get; private set; } = null!;

    public static Result<ItemOption, Error> Create(string value, Guid groupId)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            return DomainErrors.MenuItemOption.OptionValueIsEmpty;
        }
        
        return new ItemOption(value,  groupId);
    }
    
    protected override IEnumerable<object> GetEqualityComponents()
    {
        yield return Id;
    }
}