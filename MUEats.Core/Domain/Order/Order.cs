using CSharpFunctionalExtensions;
using MUEats.Core.Domain.Order.ValueObjects; 
using MUEats.Core.Primitives.ValueObjects;

namespace MUEats.Core.Domain.Order;

public class Order
{
    public Order()
    {
        
    }
    
    private Order(Money price, Address address, OrderStatus status, Guid userId, Guid restaurantId, Guid courierId, DateTime orderDate, DateTime deliverBefore, List<OrderItem> orderItems)
    {
        Id = Guid.NewGuid();
        Price = price;
        Address = address;
        Status = status;
        UserId = userId;
        RestaurantId = restaurantId;
        CourierId = courierId;
        OrderDate = orderDate;
        DeliverBefore = deliverBefore;
        OrderItems = orderItems;
    }

    public Guid Id  { get; init; }

    public Money Price { get; init; } = null!;

    public Address Address { get; private set; } = null!;
    
    public OrderStatus Status { get; private set; }
    
    public Guid UserId { get; init; }
    
    public Guid RestaurantId { get; init; }
    
    public Guid CourierId { get; init; }
    
    public DateTime OrderDate { get; init; }
    
    public DateTime DeliverBefore { get; private set; }
    
    public DateTime DeliveredAt { get; init; }

    public List<OrderItem> OrderItems { get; private set; } = [];

    public static Result<Order> Create(Money price, Address address, OrderStatus status, Guid userId, Guid restaurantId, Guid courierId, DateTime orderDate, DateTime deliverBefore, List<OrderItem> orderItems)
    {
        return new Order(price, address, status, userId, restaurantId, courierId, orderDate, deliverBefore, orderItems);
    }
}