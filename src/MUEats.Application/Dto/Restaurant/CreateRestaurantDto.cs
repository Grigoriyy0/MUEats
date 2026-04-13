using System.ComponentModel.DataAnnotations;

namespace MUEats.Application.Dto.Restaurant;

public class CreateRestaurantDto
{
    [Required(ErrorMessage = "Restaurant name is required")]
    public string Name { get; set; }
    
    public string? Description { get; set; }
    
    [Required(ErrorMessage = "Address is required")]
    public string Address { get; set; }
    
    public TimeSpan OpeningHours { get; set; }
    
    public TimeSpan ClosingHours { get; set; }
}