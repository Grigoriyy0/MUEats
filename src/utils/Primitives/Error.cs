using CSharpFunctionalExtensions;

namespace Primitives;

public class Error : ValueObject
{
    public Error()
    {
        
    }
    
    private Error(string value, string message, ErrorType type)
    {
        Value = value;
        Message = message;
        Type = type;
    }
    
    public string Value { get; private set; }
    
    public string Message { get; private set; }
    
    public ErrorType Type { get; private set; }

    public static Error Create(string value, string message, ErrorType type)
    {
        return new Error(value, message, type);
    }
    
    protected override IEnumerable<object> GetEqualityComponents()
    {
        yield return Value;
        yield return Message;
        yield return Type;
    }
}