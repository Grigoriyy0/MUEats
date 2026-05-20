using CSharpFunctionalExtensions;

namespace MUEats.Core.Primitives.ValueObjects;

public class Address : ValueObject
{
    private Address(string district, string street, string building, string apartment)
    {
        District = district;
        Street = street;
        Building = building;
        Apartment = apartment;
    }

    public string District { get; set; }
    
    public string Street { get; set; }
    
    public string Building { get; set; }
    
    public string? Apartment { get; set; }

    public static Result<Address> Create(string district, string street, string building, string apartment)
    {
        var errList = new List<string>();
        
        if (string.IsNullOrWhiteSpace(district))
        {
            errList.Add("District cannot be empty");
        }

        if (string.IsNullOrWhiteSpace(street))
        {
            errList.Add("Street cannot be empty");
        }

        if (string.IsNullOrWhiteSpace(building))
        {
            errList.Add("Building cannot be empty");
        }
        
        return errList.Count != 0 ? Result.Failure<Address>(string.Join(';', errList)) : new Address(district, street, building, apartment);
    }

    public string GetFullAddress()
    {
        var parts = new List<string>
        {
            District,
            $"{Street} st.",
            $"{Building} bldg.",
        };

        if (!string.IsNullOrWhiteSpace(Apartment))
        {
            parts.Add($"{Apartment} fl");
        }

        return string.Join(',', parts);
    }
    
    protected override IEnumerable<object> GetEqualityComponents()
    {
        yield return District;
        yield return Street;
        yield return Building;
    }
}