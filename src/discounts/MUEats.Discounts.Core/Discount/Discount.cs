namespace MUEats.Discounts.Core.Discount;

public class Discount
{
    public Guid Id { get; init; }
    
    public Guid RestaurantId { get; init; }
    
    public Guid? FoodItemId { get; init; }
    
    public decimal Value { get; private set; }

    public DiscountType Type { get; init; }
    
    public DiscountPriority Priority { get; init; }
    
    public List<IdentityRole> TargetedRoles { get; set; } = [];
    
    public DateTime ActiveFrom { get; init; }
    
    public DateTime? ActiveBefore { get; private set; }
    
    public bool IsActive { get; private set; }
    
    public int? MaxUsageCount { get; init; }
    
    public decimal? MinOrderValue { get; private set; }
}