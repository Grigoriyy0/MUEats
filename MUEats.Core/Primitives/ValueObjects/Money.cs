using CSharpFunctionalExtensions;

namespace MUEats.Core.Primitives.ValueObjects;

public class Money : ValueObject
{
    public decimal Value { get; private set; }
    
    public string Currency { get; private set; }
    
    protected override IEnumerable<object> GetEqualityComponents()
    {
        yield return Value;
        yield return Currency;
    }
}