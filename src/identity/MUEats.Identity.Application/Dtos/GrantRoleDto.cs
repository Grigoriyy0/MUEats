namespace MUEats.Identity.Application.Dtos;

public sealed record GrantRoleDto
{
    public Guid UserId { get; init; }
    
    public Guid RoleId { get; init; }
    
    public List<AttributeDto>? Attributes { get; init; }
}