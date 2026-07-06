using MUEats.Application.Dto.Role;

namespace MUEats.Application.Dto.User;

public class UserDto
{
    public Guid Id { get; set; }
    
    public string Email { get; set; }
    
    public string FirstName { get; set; }
    
    public string LastName { get; set; }
    
    public List<RoleDto> Roles { get; set; }
}