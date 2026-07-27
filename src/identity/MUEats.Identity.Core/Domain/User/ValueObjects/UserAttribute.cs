using CSharpFunctionalExtensions;
using Primitives;

namespace MUEats.Identity.Core.Domain.User.ValueObjects;

public class UserAttribute : ValueObject
{
    public UserAttribute()
    {
        
    }
    
    private UserAttribute(string key, string value)
    {
        Key = key;
        Value = value;
    }
    
    public string Key { get; init; }
    
    public string Value { get; init; }

    public static Result<UserAttribute, Error> Create(string key, string value)
    {
        if (string.IsNullOrWhiteSpace(key))
        {
            return DomainErrors.User.UserAttributeKeyIsEmpty;
        }

        if (string.IsNullOrWhiteSpace(value))
        {
            return DomainErrors.User.UserAttributeValueIsEmpty;
        }

        key = key.Trim().ToLowerInvariant();
        value = value.Trim();
        
        return new UserAttribute(key, value);
    }
    
    protected override IEnumerable<object> GetEqualityComponents()
    {
        yield return Key;
        yield return Value;
    }
}