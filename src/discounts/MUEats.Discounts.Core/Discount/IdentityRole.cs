namespace MUEats.Discounts.Core.Discount;

public class IdentityRole
{
    public Guid Id { get; set; }
    
    public Guid DiscountId { get; set; }
    
    public string RoleName { get; set; }
    
    public Discount? Discount { get; set; }
}