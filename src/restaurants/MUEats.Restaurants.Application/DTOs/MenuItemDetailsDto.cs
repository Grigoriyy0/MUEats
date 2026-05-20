namespace MUEats.Restaurants.Application.DTOs;

public class MenuItemDetailsDto
{
    public Guid ItemId { get; set; }
    
    public Guid MenuId { get; set; }
    
    public string ItemName { get; set; }
    
    public string? ItemDescription { get; set; }
    
    public decimal ItemPrice { get; set; }

    public List<OptionsGroupDto> OptionGroups { get; set; } = [];
}