using MUEats.Discounts.Core.Discount;

namespace MUEats.Discounts.Application.Dtos;

public sealed record CreateDiscountDto : IRestaurantRelatedRequest
{
    public Guid RestaurantId { get; set; }
    
    public Guid? FoodItemId { get; set; }
    
    public decimal Value { get; set; }
    
    public DiscountType Type { get; set; }
    
    public List<string> TargetRoles { get; set; } = [];
    
    public DateTime ActiveFrom { get; set; }
    
    public DateTime? ActiveBefore { get; set; }
    
    public int? MaxUsageCount { get; set; }
    
    public decimal? MinOrderValue { get; set; }
}