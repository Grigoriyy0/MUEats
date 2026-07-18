namespace MUEats.Application.Dto.Role;

public sealed record RoleDto
{
    public Guid Id { get; set; }
    
    public string Name { get; set; }
    
    public List<string> Requirements { get; set; }
}