namespace MUEats.Restaurants.Application.DTOs;

public class ItemOptionDto
{
    public Guid Id { get; set; }
    
    public string Value { get; set; }
    
    public decimal? AdditionalPrice { get; set; }
}