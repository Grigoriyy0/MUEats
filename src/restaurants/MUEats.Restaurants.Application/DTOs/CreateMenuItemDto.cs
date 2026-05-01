namespace MUEats.Restaurants.Application.DTOs;

public class CreateMenuItemDto
{
    public Guid MenuId { get; set; }
    
    public string ItemName { get; set; }
    
    public string? ItemDescription { get; set; }
    
    public decimal ItemPrice { get; set; }
    
    public Guid? CategoryId { get; set; }
}