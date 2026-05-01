using System.ComponentModel.DataAnnotations;

namespace MUEats.Restaurants.Application.DTOs;

public sealed record CreateRestaurantDto
{
    [Required]
    [StringLength(128)]
    public string Name { get; set; } = null!;
    
    public string? Description { get; set; }
    
    public TimeSpan OpeningTime { get; set; }
    
    public TimeSpan ClosingTime { get; set; }
    
    [Required]
    [StringLength(128)]
    public string Address { get; set; }
}