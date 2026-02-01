using MUEats.Core.Primitives;

namespace MUEats.Core.Domain.Restaurant.Entities;

public class FoodItem
{
    public Guid Id { get; set; }

    public string Name { get; set; } = null!;
    
    public string? Description { get; set; }
    
    public Money Price { get; set; }
    
    public bool IsAvailable { get; set; }
    
    public Guid RestaurantId { get; set; }
    
    public Restaurant? Restaurant { get; set; }
}