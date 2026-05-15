namespace MUEats.Restaurants.Application.DTOs;

public class OrderItemDto
{
    public Guid Id { get; set; }
    
    public string ItemName { get; set; }
    
    public decimal Price { get; set; }
    
    public int Quantity { get; set; }
}