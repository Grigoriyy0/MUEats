namespace MUEats.Application.Dto.Order;

public sealed record OrderItemDto
{
    public Guid Id { get; set; }
    
    public string ItemName { get; set; }
    
    public decimal Price { get; set; }
    
    public int Quantity { get; set; }
}