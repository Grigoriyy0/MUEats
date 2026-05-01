namespace MUEats.Restaurants.Application.DTOs;

public class CreateRestaurantDto
{
    public string Name { get; set; }
    
    public string? Description { get; set; }
    
    public TimeSpan OpeningTime { get; set; }
    
    public TimeSpan ClosingTime { get; set; }
    
    public string Address { get; set; }
}