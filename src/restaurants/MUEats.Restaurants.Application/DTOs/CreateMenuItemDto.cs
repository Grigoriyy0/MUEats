using System.ComponentModel.DataAnnotations;

namespace MUEats.Restaurants.Application.DTOs;

public sealed record CreateMenuItemDto
{
    [Required]
    [StringLength(128)]
    public string ItemName { get; set; }
    
    public string? ItemDescription { get; set; }
    
    [Required]
    [Range(0, double.MaxValue)]
    public decimal ItemPrice { get; set; }
    
    public Guid CategoryId { get; set; }
}