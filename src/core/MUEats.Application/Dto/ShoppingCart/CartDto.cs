namespace MUEats.Application.Dto.ShoppingCart;

public sealed record CartDto
{
    public Guid Id { get; set; }
    
    public Guid RestaurantId { get; set; }
    
    public string RestaurantName { get; set; }
    
    public Guid UserId { get; set; }

    public List<CartItemDto> Items { get; set; } = [];
}