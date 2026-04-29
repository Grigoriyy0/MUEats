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
        string address)
    {
        Id = Guid.NewGuid();
        Name = name;
        Description = description;
        BusinessHours = businessHours;
        Address = address;
    }

    public Guid Id { get; init; }

    public string Name { get; private set; } = null!;
    
    public string? Description { get; private set; }
    
    public BusinessHours BusinessHours { get; private set; }

    public string Address { get; private set; } = null!; 
    
    public Guid MenuId { get; private set; }

    public static Result<Restaurant, Error> Create(string name, 
        string? description, 
        BusinessHours businessHours,
        string address)
    {
        //todo validation
        return new Restaurant(name,  description, businessHours, address);
    }

    public void UpdateMenuId(Guid menuId)
    {
        MenuId = menuId;
    }

    public UnitResult<Error> UpdateName(string name)
    {
        if (string.IsNullOrEmpty(name))
        {
            // return error
        }

        Name = name;

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