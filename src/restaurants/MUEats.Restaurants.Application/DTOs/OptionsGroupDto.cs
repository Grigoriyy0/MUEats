namespace MUEats.Restaurants.Application.DTOs;

public class OptionsGroupDto
{
    public Guid GroupId { get; set; }
    
    public Guid MenuItemId { get; set; }
    
    public List<ItemOptionDto> ItemOpions { get; set; }
}