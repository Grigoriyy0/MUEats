namespace MUEats.Discounts.Core.Discount;

public class UserUsageCount
{
    public Guid Id { get; set; }
    
    public Guid UserId { get; set; }
    
    public Guid DiscountId { get; set; }
    
    public int Count { get; set; }
}