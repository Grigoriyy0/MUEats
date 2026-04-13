namespace MUEats.Application.Dto.Restaurant;

public class RestaurantDto
{
    public Guid Id { get; init; }

    public string Name { get; set; } = null!;

    public string Address { get; set; } = null!;

    public TimeSpan OpeningHours { get; set; }

    public TimeSpan ClosingHours { get; set; }

    public decimal MinimumOrderAmount { get; set; }

    public List<FoodItemDto> FoodItems { get; set; } = [];
}