namespace MUEats.Core.Domain.Order.ValueObjects;

public enum OrderStatus
{
    Created,
    Accepted,
    Preparing,
    Prepared,
    Completed,
    Cancelled,
    Rejected,
    Failed
}