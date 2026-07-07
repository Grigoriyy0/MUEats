namespace MUEats.Discounts.Application.Dtos;

public sealed record CreateDiscountDto
{
    public Guid RestaurantId { get; set; }
    
    public Guid FoodItemId { get; set; }
    
    public decimal? Value { get; set; }

    public decimal? PercentageValue { get; set; }

    public List<Guid> TargetRoleIds { get; set; } = [];
}