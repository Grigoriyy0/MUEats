using CSharpFunctionalExtensions;

namespace MUEats.Core.Primitives.ValueObjects;

public class Error : ValueObject
{
    public Error()
    {
        
    }
    
    private Error(string value, string message)
    {
        Value = value;
        Message = message;
    }
    
    public string Value { get; private set; }
    
    public string Message { get; private set; }

    public static Error Create(string value, string message)
    {
        return new Error(value, message);
    }
    
    protected override IEnumerable<object> GetEqualityComponents()
    {
        yield return Value;
        yield return Message;
    }
}