using MUEats.Discounts.Core.Discount;

namespace MUEats.Discounts.Application.Dtos;

public sealed record CreateDiscountDto
{
    public Guid RestaurantId { get; set; }
    
    public Guid? FoodItemId { get; set; }
    
    public decimal Value { get; set; }
    
    public DiscountType Type { get; set; }
    
    public List<string> TargetRoles { get; set; } = [];
}