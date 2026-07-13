namespace MUEats.Discounts.Application.Queries;

public sealed record GetDiscountsQuery
{
    public Guid? RestaurantId { get; set; } = null;

    public Guid? FoodItemId { get; set; } = null;

    public List<string> Roles { get; set; } = [];
}