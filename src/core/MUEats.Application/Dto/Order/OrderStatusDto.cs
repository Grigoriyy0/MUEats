namespace MUEats.Application.Dto.Order;

public class OrderStatusDto
{
    public Guid OrderId { get; set; }
    
    public string OrderStatus { get; set; }
    
    public string DeliveryStatus { get; set; }
}