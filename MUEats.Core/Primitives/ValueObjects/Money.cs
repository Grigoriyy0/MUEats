using CSharpFunctionalExtensions;

namespace MUEats.Core.Primitives.ValueObjects;

public class Money : ValueObject
{
    private Money(decimal value, string currency)
    {
        Value = value;
        Currency = currency;
    }
    
    public decimal Value { get; private set; }
    
    public string Currency { get; private set; }


    public static Result<Money> Create(decimal value, string currency)
    {
        return new Money(value, currency);
    }
    
    public string GetValue()
    {
        return $"{Currency} {Value}";
    }
    
    protected override IEnumerable<object> GetEqualityComponents()
    {
        yield return Value;
        yield return Currency;
    }
}