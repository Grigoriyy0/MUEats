namespace MUEats.Restaurants.Core.Projections.Order;

public class OrderItemSnapshot
{
    public Guid Id { get; set; }
    
    public Guid FoodItemId { get; set; }
    
    public Guid RestaurantId { get; set; }
    
    public Guid OrderId { get; set; }
    
    public int Quantity { get; set; }
    
    public decimal Price { get; set; }
    
    public OrderSnapshot? Order { get; set; }
}