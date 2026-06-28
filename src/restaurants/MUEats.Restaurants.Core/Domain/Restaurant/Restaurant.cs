using CSharpFunctionalExtensions;
using MUEats.Restaurants.Core.Domain.Restaurant.ValueObjects;
using Primitives;

namespace MUEats.Restaurants.Core.Domain.Restaurant;

public class Restaurant
{
    private Restaurant(
        string name, 
        string? description, 
        BusinessHours businessHours,
        string address,
        double latitude,
        double longitude)
    {
        Id = Guid.NewGuid();
        Name = name;
        Description = description;
        BusinessHours = businessHours;
        Address = address;
        Latitude = latitude;
        Longitude = longitude;
    }

    public Guid Id { get; init; }

    public string Name { get; private set; }
    
    public string? Description { get; private set; }
    
    public BusinessHours BusinessHours { get; private set; }

    public string Address { get; private set; } 
    
    public double Latitude { get; private set; }
    public double Longitude { get; private set; }
    
    public Guid MenuId { get; private set; }

    public static Result<Restaurant, Error> Create(string name, 
        string? description, 
        BusinessHours businessHours,
        string address,
        double latitude,
        double longitude)
    {
        if (string.IsNullOrWhiteSpace(name))
        {
            return DomainErrors.Restaurant.NameIsEmpty;
        }

        if (string.IsNullOrWhiteSpace(address))
        {
            return DomainErrors.Restaurant.AddressIsEmpty;
        }
        
        return new Restaurant(name, description, businessHours, address, latitude, longitude);
    }

    public void AddMenuId(Guid menuId)
    {
        MenuId = menuId;
    }

    public UnitResult<Error> UpdateName(string name)
    {
        if (string.IsNullOrWhiteSpace(name))
        {
            return DomainErrors.Restaurant.NameIsEmpty;
        }

        Name = name;

        return UnitResult.Success<Error>();
    }

    public UnitResult<Error> UpdateAddress(string address)
    {
        if (string.IsNullOrWhiteSpace(address))
        {
            return DomainErrors.Restaurant.AddressIsEmpty;
        }
        
        Address = address;
        
        return UnitResult.Success<Error>();
    }

    public void UpdateDescription(string description)
    {
        Description = description;
    }

    public void UpdateBusinessHours(BusinessHours businessHours)
    {
        BusinessHours = businessHours;
    }
}