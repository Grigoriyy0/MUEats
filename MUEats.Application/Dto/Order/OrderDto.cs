using MUEats.Core.Domain.Order.ValueObjects;

namespace MUEats.Application.Dto.Order;

public class OrderDto
{
    public Guid Id { get; set; }
    
    public decimal TotalPrice { get; set; }
    
    public string RestaurantName { get; set; }
    
    public string Address { get; set; }

    public List<OrderItemDto> OrderItems { get; set; } = [];
}