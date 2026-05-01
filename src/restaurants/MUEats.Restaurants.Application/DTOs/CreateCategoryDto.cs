namespace MUEats.Restaurants.Application.DTOs;

public class CreateCategoryDto
{
    public Guid MenuId { get; set; }
    
    public string Name { get; set; }
    
    public string? Description { get; set; }
}