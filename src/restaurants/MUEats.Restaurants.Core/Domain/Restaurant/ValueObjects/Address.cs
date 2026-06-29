using CSharpFunctionalExtensions;
using Primitives;

namespace MUEats.Restaurants.Core.Domain.Restaurant.ValueObjects;

public class Address : ValueObject
{
    private Address(string addressLine, double latitude, double longitude)
    {
        AddressLine = addressLine;
        Latitude = latitude;
        Longitude = longitude;
    }

    public string AddressLine { get; private set; }
    
    public double Latitude { get; private set; }
    
    public double Longitude { get; private set; }
    
    public static Result<Address, Error> Create(string addressLine, double latitude, double longitude)
    {
        if (string.IsNullOrWhiteSpace(addressLine))
        {
            return DomainErrors.Restaurant.AddressIsEmpty;
        }
        
        return new Address(addressLine, latitude, longitude);
    }
    
    protected override IEnumerable<object> GetEqualityComponents()
    {
        yield return Latitude;
        yield return Longitude;
    }
}