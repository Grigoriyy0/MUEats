namespace MUEats.Application.Dto.ShoppingCart;

public class CartItemDto
{
    public Guid Id { get; set; }
    
    public Guid FoodItemId { get; init; }
    
    public string FoodItemName { get; init; }

    public Guid CartId { get; init; }

    public decimal Price { get; init; }

    public int Quantity { get; set; }
}