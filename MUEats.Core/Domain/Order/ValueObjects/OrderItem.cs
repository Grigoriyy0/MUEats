using MUEats.Core.Primitives.ValueObjects;

namespace MUEats.Core.Domain.Order.ValueObjects;

public class OrderItem
{
    public Guid Id { get; init; }

    public Guid OrderId { get; init; }

    public string Name { get; init; } = null!;

    public decimal Price { get; init; }

    public int Quantity { get; set; }

    public Order? Order { get; set; }
}