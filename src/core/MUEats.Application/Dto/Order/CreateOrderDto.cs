namespace MUEats.Application.Dto.Order;

public sealed record CreateOrderDto
{
    public DateTime? PickUpTime { get; set; }
}