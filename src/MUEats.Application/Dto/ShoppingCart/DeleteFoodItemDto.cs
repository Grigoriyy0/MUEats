namespace MUEats.Application.Dto.ShoppingCart;

public class DeleteFoodItemDto
{
    public Guid CartId { get; set; }
    
    public Guid FoodItemId { get; set; }
}