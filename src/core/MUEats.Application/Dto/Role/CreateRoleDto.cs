namespace MUEats.Application.Dto.Role;

public sealed record CreateRoleDto
{
    public string RoleName { get; set; }
    
    public List<string>? RequiredAttributes { get; set; }
}