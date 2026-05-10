namespace MUEats.Application.Dto.ShoppingCart;

public class AddFoodItemDto
{
    public Guid RestaurantId { get; set; }
    
    public Guid ItemId { get; set; }
    
    public string RestaurantName { get; set; }
    
    public string ItemName { get; set; }
    
    public decimal ItemPrice { get; set; }
}