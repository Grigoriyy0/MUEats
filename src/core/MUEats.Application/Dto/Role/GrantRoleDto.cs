namespace MUEats.Application.Dto.Role;

public sealed record GrantRoleDto
{
    public Guid UserId { get; set; }
    
    public Guid RoleId { get; set; }
    
    public List<AttributeDto>? Attributes { get; set; }
}