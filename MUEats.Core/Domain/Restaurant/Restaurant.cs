using MUEats.Core.Domain.Restaurant.Entities;
using MUEats.Core.Primitives;

namespace MUEats.Core.Domain.Restaurant;

public class Restaurant
{
    public Guid Id { get; set; }

    public string Name { get; set; } = null!;
    
    public Address Address { get; set; }
    
    public DateTime OpeningHours { get; set; }
    
    public DateTime ClosingHours { get; set; }
    
    public decimal MinimumOrderAmount { get; set; }
    
    public Guid ManagerId { get; set; }

    public List<FoodItem> FoodItems { get; set; } = [];
}