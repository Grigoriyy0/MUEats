namespace MUEats.Discounts.Core.Discount;

public class Discount
{
    public Guid Id { get; set; }
    
    public Guid RestaurantId { get; set; }
    
    public Guid? FoodItemId { get; set; }
    
    public decimal Value { get; set; }

    public DiscountType Type { get; set; }
    
    public DiscountPriority Priority { get; set; }
    
    public List<IdentityRole> TargetedRoles { get; set; } = [];
    
    public DateTime ActiveFrom { get; set; }
    
    public DateTime? ActiveBefore { get; set; }
    
    public bool IsActive { get; set; }
    
    public int? MaxUsageCount { get; set; }
    
    public decimal? MinOrderValue { get; set; }
}