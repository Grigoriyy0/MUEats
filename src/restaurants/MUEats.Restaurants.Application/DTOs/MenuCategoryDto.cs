namespace MUEats.Restaurants.Application.DTOs;

public class MenuCategoryDto
{
    public Guid Id { get; set; }
    
    public string Name { get; set; }
    
    public List<MenuItemDto> MenuItems { get; set; }
}