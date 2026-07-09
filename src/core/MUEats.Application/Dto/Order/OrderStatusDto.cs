namespace MUEats.Application.Dto.Order;

public sealed record OrderStatusDto
{
    public Guid OrderId { get; set; }
    
    public string OrderStatus { get; set; }
}