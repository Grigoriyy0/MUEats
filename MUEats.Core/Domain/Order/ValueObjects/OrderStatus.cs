namespace MUEats.Core.Domain.Order.ValueObjects;

public enum OrderStatus
{
    Created,
    Accepted,
    Prepared,
    CourierFound,
    Delivery,
    Completed,
    Cancelled,
    Failed
}