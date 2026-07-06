namespace Primitives;

public class GeneralError
{
    public static Error ValueIsIncorrect(string valueName)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(valueName);
        
        return Error.Create(valueName, $"{valueName} is incorrect", ErrorType.Validation);
    }

    public static Error ValueIsIncorrect(string valueName, string description)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(valueName);
        ArgumentException.ThrowIfNullOrWhiteSpace(description);

        return Error.Create(valueName, description, ErrorType.Validation);
    }
    
    public static Error AlreadyExists(string value, string description)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(description);
        ArgumentException.ThrowIfNullOrWhiteSpace(value);

        return Error.Create(value, description, ErrorType.AlreadyExists);
    }

    public static Error NotFound(string value)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(value);

        return Error.Create(value, $"{value} is not found", ErrorType.NotFound);
    }
}