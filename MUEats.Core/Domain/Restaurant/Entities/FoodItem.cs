using CSharpFunctionalExtensions;
using MUEats.Core.Primitives.ValueObjects;

namespace MUEats.Core.Domain.Restaurant.Entities;

public class FoodItem
{
    public FoodItem()
    {
        
    }
    
    private FoodItem(string name, string? description, Money price, bool isAvailable, Guid restaurantId)
    {
        Id = Guid.NewGuid();
        Name = name;
        Description = description;
        Price = price;
        IsAvailable = isAvailable;
        RestaurantId = restaurantId;
    }

    public Guid Id { get; init; }

    public string Name { get; private set; }
    
    public string? Description { get; private set; }

    public Money Price { get; private set; }
    
    public bool IsAvailable { get; private set; }
    
    public Guid RestaurantId { get; init; }
    
    public Restaurant? Restaurant { get; set; }

    public static Result<FoodItem> Create(string name, string? description, Money price, bool isAvailable, Guid restaurantId)
    {
        return new FoodItem(name, description, price, isAvailable, restaurantId);
    }
}