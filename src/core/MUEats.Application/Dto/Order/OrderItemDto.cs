namespace MUEats.Application.Dto.Order;

public class OrderItemDto
{
    public Guid Id { get; set; }
    
    public string ItemName { get; set; }
    
    public decimal Price { get; set; }
}