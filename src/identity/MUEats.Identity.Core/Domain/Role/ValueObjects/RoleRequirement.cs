using CSharpFunctionalExtensions;
using Primitives;

namespace MUEats.Identity.Core.Domain.Role.ValueObjects;

public class RoleRequirement : ValueObject
{
    public RoleRequirement()
    {
        
    }

    private RoleRequirement(string valueName)
    {
        ValueName = valueName;
    }

    public string ValueName { get; init; }

    public static Result<RoleRequirement, Error> Create(string valueName)
    {
        if (string.IsNullOrWhiteSpace(valueName))
        {
            return DomainErrors.Role.RoleRequirementValueNameIsEmpty;
        }

        valueName = valueName.Trim().ToLowerInvariant();

        return new RoleRequirement(valueName);
    }
    
    protected override IEnumerable<object> GetEqualityComponents()
    {
        yield return ValueName;
    }
}