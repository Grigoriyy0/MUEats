namespace MUEats.Restaurants.Core.Projections.Order;

public class OrderSnapshot
{
    public Guid Id { get; set; }
    
    public Guid OrderId { get; set; }
    
    public DateTime OrderDate { get; set; }
    
    public Guid RestaurantId { get; set; }
    
    public OrderStatus Status { get; set; }
    
    public Guid? LockId { get; set; }
    
    public int RetryCount { get; set; }
    
    public string? LastError { get; set; }
    
    public DateTime? NextAttemptAt { get; set; }
    
    public DateTime CreatedAt { get; set; }
    
    public DateTime? UpdatedAt { get; set; }
}