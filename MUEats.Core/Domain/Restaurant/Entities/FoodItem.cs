using MUEats.Core.Primitives.ValueObjects;

namespace MUEats.Core.Domain.Restaurant.Entities;

public class FoodItem
{
    public Guid Id { get; init; }

    public string Name { get; set; }

    public string? Description { get; set; }

    public decimal Price { get; set; }

    public bool IsAvailable { get; set; }

    public Guid RestaurantId { get; init; }

    public Restaurant? Restaurant { get; set; }
}