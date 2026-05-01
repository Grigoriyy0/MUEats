namespace MUEats.Restaurants.Application.DTOs;

public sealed record MenuDto
{
    public Guid Id { get; set; }
    
    public Guid RestaurantId { get; set; }
    
    public List<MenuCategoryDto> MenuCategories { get; set; }
}