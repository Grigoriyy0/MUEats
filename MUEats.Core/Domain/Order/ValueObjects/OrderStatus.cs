namespace MUEats.Core.Domain.Order.ValueObjects;

public enum OrderStatus
{
    Created,
    Accepted,
    Prepared,
    Completed,
    Cancelled,
    Failed
}