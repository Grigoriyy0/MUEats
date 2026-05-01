namespace MUEats.Restaurants.Infrastructure.ExternalServices.Api;

public class OrderSnapshot
{
    public Guid Id { get; set; }
    
    public Guid OrderId { get; set; }
    
    public DateTime OrderDate { get; set; }
    
    public Guid RestaurantId { get; set; }
    
    public OrderStatus Status { get; set; }
}