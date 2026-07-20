using System.Text.RegularExpressions;
using CSharpFunctionalExtensions;
using Primitives;

namespace MUEats.Identity.Core.Domain.User.ValueObjects;

public partial class EmailAddress : ValueObject
{
    private static readonly Regex Regex = EmailRegex();
    
    public string Value { get; private set; }
    
    private EmailAddress(string value)
    {
        Value = value;
    }

    public static Result<EmailAddress, Error> Create(string emailAddress)
    {
        if (string.IsNullOrWhiteSpace(emailAddress))
        {
            return DomainErrors.User.EmailIsEmpty;
        }
        
        var normalizedEmail = emailAddress.Trim().ToLowerInvariant();

        if (!Regex.IsMatch(normalizedEmail))
        {
            return DomainErrors.User.EmailDoesNotMatch;
        }
        
        return new EmailAddress(normalizedEmail);
    }
    
    protected override IEnumerable<object> GetEqualityComponents()
    {
        yield return Value;
    }
    
    public override string ToString() => Value;
    
    [GeneratedRegex(@"^[^@\s]+@[^@\s]+\.[^@\s]+$")]
    private static partial Regex EmailRegex();
}