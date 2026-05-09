using Microsoft.Extensions.Options;
using MUEats.Application.Ports;
using MUEats.Infrastructure.Options;

namespace MUEats.Infrastructure.Adapters.Services;

public class PasswordValidator : IPasswordValidator
{
    private readonly PasswordValidatorOptions _options;

    public PasswordValidator(IOptions<PasswordValidatorOptions> options)
    {
        _options = options.Value;
    }

    public bool Validate(string password)
    {
        if (password.Length > _options.MaxLength || password.Length < _options.MinLength)
        {
            return false;
        }
        
        //here it is possible to add any other validation requirements such as special chars and etc.

        return true;
    }
}