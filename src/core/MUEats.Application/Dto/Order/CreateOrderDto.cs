namespace MUEats.Application.Dto.Order;

public class CreateOrderDto
{
    public Guid UserId { get; set; }

    public string Address { get; set; } = null!;
}