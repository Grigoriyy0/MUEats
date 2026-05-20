using System.ComponentModel.DataAnnotations;

namespace MUEats.Restaurants.Application.DTOs;

public sealed record CreateCategoryDto
{
    [Required]
    [StringLength(128)]
    public string Name { get; set; }
    
    public string? Description { get; set; }
}