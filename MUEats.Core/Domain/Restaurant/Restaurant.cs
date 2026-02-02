using MUEats.Core.Domain.Restaurant.Entities;
using MUEats.Core.Primitives.ValueObjects;

namespace MUEats.Core.Domain.Restaurant;

public class Restaurant
{
    public Guid Id { get; init; }

    public string Name { get; set; } = null!;

    public string Address { get; set; } = null!;

    public TimeSpan OpeningHours { get; set; }

    public TimeSpan ClosingHours { get; set; }

    public decimal MinimumOrderAmount { get; set; }

    public Guid ManagerId { get; set; }

    public List<FoodItem> FoodItems { get; set; } = [];
}