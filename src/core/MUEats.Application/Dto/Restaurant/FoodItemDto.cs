namespace MUEats.Application.Dto.Restaurant;

public class FoodItemDto
{
    public Guid Id { get; init; }

    public string Name { get; set; }

    public string? Description { get; set; }

    public decimal Price { get; set; }

    public bool IsAvailable { get; set; }

    public Guid RestaurantId { get; init; }
}