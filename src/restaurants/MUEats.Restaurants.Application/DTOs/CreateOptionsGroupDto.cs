namespace MUEats.Restaurants.Application.DTOs;

public sealed record CreateOptionsGroupDto
{
    public string Name { get; set; }
    
    public string? Description { get; set; }
}