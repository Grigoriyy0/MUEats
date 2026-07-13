namespace MUEats.Discounts.Application.Dtos;

public sealed record DiscountDto
{
    public Guid Id { get; init; }
    
    public Guid? FoodItemId { get; init; }
    
    public decimal Value { get; init; }
    
    public string Type { get; init; }
}