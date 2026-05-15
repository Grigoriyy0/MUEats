namespace MUEats.Restaurants.Core.Projections.Order;

public enum OrderStatus
{
    Created,
    Pending,
    Accepted,
    Preparing,
    Prepared,
    Completed,
    Rejected
}