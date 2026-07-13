namespace MUEats.Discounts.Application.Queries;

public sealed record GetDiscountsQuery
{
    public Guid RestaurantId { get; set; }

    public List<Guid> FoodItemIds { get; set; } = [];
    
    public decimal CartSubtotal { get; set; }

    public List<string> Roles { get; set; } = [];
}