namespace MUEats.Discounts.Core.Discount;

public class Discount
{
    public Guid Id { get; set; }
    
    public Guid RestaurantId { get; set; }
    
    public Guid FoodItemId { get; set; }
    
    public decimal Value { get; set; }

    public DiscountType Type { get; set; }
    
    public List<IdentityRole> TargetedRoles { get; set; } = [];
    
    public DateTime CreatedAt { get; set; }
}