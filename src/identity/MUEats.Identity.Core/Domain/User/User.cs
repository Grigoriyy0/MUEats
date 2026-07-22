using CSharpFunctionalExtensions;
using MUEats.Identity.Core.Domain.User.ValueObjects;
using Primitives;

namespace MUEats.Identity.Core.Domain.User;

public class User
{
    public User()
    {
        
    }
    
    private User(string firstName, 
        string lastName, 
        EmailAddress emailAddress, 
        string passwordHash)
    {
        Id = Guid.NewGuid();
        FirstName = firstName;
        LastName = lastName;
        EmailAddress = emailAddress;
        PasswordHash = passwordHash;
    }

    public Guid Id { get; private set; }

    public string FirstName { get; private set; } = null!;

    public string LastName { get; private set; } = null!;

    public EmailAddress EmailAddress { get; private set; } = null!;

    public string PasswordHash { get; private set; } = null!;

    private readonly List<Guid> _roleIds = new();

    public IReadOnlyCollection<Guid> RoleIds => _roleIds.AsReadOnly();

    private readonly List<UserAttribute> _attributes = new();

    public IReadOnlyCollection<UserAttribute> Attributes => _attributes.AsReadOnly();

    public static Result<User, Error> Create(string firstName,
        string lastName,
        string emailAddress,
        string passwordHash)
    {
        if (string.IsNullOrWhiteSpace(firstName) || string.IsNullOrWhiteSpace(lastName))
        {
            return DomainErrors.User.NameIsEmpty;
        }

        var emailResult = EmailAddress.Create(emailAddress);

        if (emailResult.IsFailure)
        {
            return emailResult.Error;
        }

        if (string.IsNullOrWhiteSpace(passwordHash))
        {
            return DomainErrors.User.PasswordHashIsEmpty;
        }
        
        return new User(firstName, lastName, emailResult.Value, passwordHash);
    }

    public Result AddRole(Guid roleId)
    {
        if (roleId == Guid.Empty)
        {
            return Result.Failure("role id is empty");
        }

        if (_roleIds.Contains(roleId))
        {
            return Result.Success();
        }

        _roleIds.Add(roleId);

        return Result.Success();
    }

    public UnitResult<Error> AddOrUpdateAttribute(string key, string value)
    {
        var attribute = _attributes.FirstOrDefault(x => x.Key == key);

        if (attribute != null)
        {
            _attributes.Remove(attribute);
        }
        
        var attributeResult = UserAttribute.Create(key, value);

        if (attributeResult.IsFailure)
        {
            return attributeResult.Error;
        }
        
        _attributes.Add(attributeResult.Value);

        return UnitResult.Success<Error>();
    }
}