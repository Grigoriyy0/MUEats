using MUEats.Core.Domain.Order.ValueObjects;

namespace MUEats.Application.Dto.Order;

public class OrderDto
{
    public Guid Id { get; set; }
    
    public Guid RestaurantId { get; set; }
    
    public decimal TotalPrice { get; set; }
    
    public string RestaurantDetails { get; set; }
    
    public string DeliveryAddress { get; set; }

    public List<OrderItemDto> OrderItems { get; set; } = [];
}