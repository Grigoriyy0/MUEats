namespace MUEats.Application.Dto.User;

public class CreateManagerDto
{
    public Guid RestaurantId { get; set; }
    
    public string UserName { get; set; }
    
    public string FirstName { get; set; }
    
    public string LastName { get; set; }
    
    public string Email { get; set; }
    
    public string Password { get; set; }
}