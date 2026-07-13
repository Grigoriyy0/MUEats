namespace MUEats.Discounts.Core;

public class IdentityRole
{
    public Guid Id { get; set; }
    
    public Guid DiscountId { get; set; }
    
    public string RoleName { get; set; }
    
    public Discount.Discount? Discount { get; set; }
}