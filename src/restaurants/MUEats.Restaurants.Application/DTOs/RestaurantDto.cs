namespace MUEats.Restaurants.Application.DTOs;

public class RestaurantDto
{
    public Guid Id { get; set; }
    
    public Guid MenuId { get; set; }
    
    public string Name { get; set; }
    
    public string? Description { get; set; }
    
    public string BusinessHours { get; set; }
    
    public string Address { get; set; }
}