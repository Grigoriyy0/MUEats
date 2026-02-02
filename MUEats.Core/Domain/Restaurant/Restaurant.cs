using CSharpFunctionalExtensions;
using MUEats.Core.Domain.Restaurant.Entities;
using MUEats.Core.Primitives.ValueObjects;

namespace MUEats.Core.Domain.Restaurant;

public class Restaurant
{
    public Restaurant()
    {
        
    }
    
    private Restaurant(string name, Address address, TimeSpan openingHours, TimeSpan closingHours, decimal minimumOrderAmount, Guid managerId)
    {
        Id = Guid.NewGuid();
        Name = name;
        Address = address;
        OpeningHours = openingHours;
        ClosingHours = closingHours;
        MinimumOrderAmount = minimumOrderAmount;
        ManagerId = managerId;
    }

    public Guid Id { get; init; }

    public string Name { get; private set; } = null!;
    
    public Address Address { get; private set; }
    
    public TimeSpan OpeningHours { get; private set; }
    
    public TimeSpan ClosingHours { get; private set; }
    
    public decimal MinimumOrderAmount { get; private set; }
    
    public Guid ManagerId { get; private set; }

    public List<FoodItem> FoodItems { get; private set; } = [];

    public static Result<Restaurant> Create(string name, Address address, TimeSpan openingHours, TimeSpan closingHours, decimal minimumOrderAmount, Guid managerId)
    {
        return new Restaurant(name, address, openingHours, closingHours,  minimumOrderAmount, managerId);
    }
}