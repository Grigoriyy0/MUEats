namespace MUEats.Identity.Application.Dtos;

public sealed record UserDto
{
    public Guid Id { get; set; }
    
    public string Email { get; set; }
    
    public string FirstName { get; set; }
    
    public string LastName { get; set; }
    
    public List<RoleDto> Roles { get; set; }
}