using CSharpFunctionalExtensions;
using MUEats.Identity.Core.Domain.Role.ValueObjects;
using Primitives;

namespace MUEats.Identity.Core.Domain.Role;

public class Role
{
    private Role(string roleName)
    {
        Id = Guid.NewGuid();
        RoleName = roleName;
    }

    public Guid Id { get; init; }

    public string RoleName { get; private set; }

    private readonly List<RoleRequirement> _requirements = new();

    public IReadOnlyCollection<RoleRequirement> Requirements => _requirements.AsReadOnly();

    public static Result<Role, Error> Create(string roleName)
    {
        if (string.IsNullOrWhiteSpace(roleName))
        {
            return DomainErrors.Role.RoleNameIsEmpty;
        }

        return new Role(roleName);
    }

    public UnitResult<Error> AddOrUpdateRequirement(string valueName)
    {
        var requirement = _requirements.FirstOrDefault(x => x.ValueName == valueName);

        if (requirement != null)
        {
            return UnitResult.Success<Error>();
        }

        var requirementResult = RoleRequirement.Create(valueName);

        if (requirementResult.IsFailure)
        {
            return requirementResult.Error;
        }
        
        _requirements.Add(requirementResult.Value);

        return UnitResult.Success<Error>();
    }

    public UnitResult<Error> UpdateRoleName(string newRoleName)
    {
        if (string.IsNullOrWhiteSpace(newRoleName))
        {
            return DomainErrors.Role.RoleNameIsEmpty;
        }

        RoleName = newRoleName;

        return UnitResult.Success<Error>();
    }
}