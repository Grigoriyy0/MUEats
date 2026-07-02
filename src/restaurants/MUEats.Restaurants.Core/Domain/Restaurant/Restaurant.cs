using CSharpFunctionalExtensions;
using MUEats.Restaurants.Core.Domain.Restaurant.ValueObjects;
using Primitives;

namespace MUEats.Restaurants.Core.Domain.Restaurant;

public class Restaurant
{
    public Restaurant()
    {
        
    }
    
    private Restaurant(
        string name, 
        string? description, 
        BusinessHours businessHours, 
        Address address)
    {
        Id = Guid.NewGuid();
        Name = name;
        Description = description;
        BusinessHours = businessHours;
        Address = address;
    }

    public Guid Id { get; init; }

    public string Name { get; private set; }
    
    public string? Description { get; private set; }
    
    public BusinessHours BusinessHours { get; private set; }

    public Address Address { get; private set; }
    
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

        var addressResult = Address.Create(address, latitude, longitude);

        if (addressResult.IsFailure)
        {
            return addressResult.Error;
        }
        
        return new Restaurant(name, description, businessHours, addressResult.Value);
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

    public UnitResult<Error> UpdateAddress(string addressLine, double latitude, double longitude)
    {
        var addressResult = Address.Create(addressLine, latitude, longitude);

        if (addressResult.IsFailure)
        {
            return addressResult.Error;
        }

        Address = addressResult.Value;
        
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