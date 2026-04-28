namespace MUEats.Application.Dto.Restaurant;

public class CreateFoodItemDto
{
    public Guid RestaurantId { get; set; }
    
    public string Name { get; set; }
    
    public string? Description { get; set; }
    
    public decimal Price { get; set; }
}