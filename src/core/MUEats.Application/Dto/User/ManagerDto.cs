namespace MUEats.Application.Dto.User;

public class ManagerDto
{
    public Guid Id { get; set; }
    
    public string UserName { get; set; }
    
    public string FirstName { get; set; }
    
    public string LastName { get; set; }
    
    public string Email { get; set; }
    
    public string Role { get; set; }
    
    public string RestaurantId { get; set; }
}