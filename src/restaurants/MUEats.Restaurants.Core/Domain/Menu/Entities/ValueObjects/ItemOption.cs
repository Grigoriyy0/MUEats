using CSharpFunctionalExtensions;
using Primitives;

namespace MUEats.Restaurants.Core.Domain.Menu.Entities.ValueObjects;

public class ItemOption : ValueObject
{
    private ItemOption(string value)
    {
        Id = Guid.NewGuid();
        Value = value;
    }
    
    public Guid Id { get; private set; }

    public string Value { get; private set; } = null!;

    internal static Result<ItemOption, Error> Create(string value)
    {
        return new ItemOption(value);
    }
    
    protected override IEnumerable<object> GetEqualityComponents()
    {
        yield return Id;
    }
}