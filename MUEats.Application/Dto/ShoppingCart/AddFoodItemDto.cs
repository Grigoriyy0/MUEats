namespace MUEats.Application.Dto.ShoppingCart;

public class AddFoodItemDto
{
    public Guid UserId { get; set; }
    
    public Guid FoodItemId { get; set; }
}