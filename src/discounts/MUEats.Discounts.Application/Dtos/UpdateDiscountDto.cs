using MUEats.Discounts.Core.Discount;

namespace MUEats.Discounts.Application.Dtos;

public sealed record UpdateDiscountDto : IRestaurantRelatedRequest
{
    public Guid Id { get; init; }
    
    public Guid RestaurantId { get; set; }
    
    public Guid? FoodItemId { get; init; }
    
    public decimal Value { get; init; }

    public DiscountType Type { get; init; }
    
    public DateTime ActiveFrom { get; init; }
    
    public DateTime? ActiveBefore { get; init; }
    
    public bool IsActive { get; init; }
    
    public int? MaxUsageCount { get; init; }
    
    public decimal? MinOrderValue { get; init; }
}