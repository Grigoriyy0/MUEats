namespace MUEats.Core.Domain.Order.ValueObjects;

public enum OrderStatus
{
    Created,
    Accepted,
    Preparation,
    Delivery,
    Completed,
    Cancelled
}