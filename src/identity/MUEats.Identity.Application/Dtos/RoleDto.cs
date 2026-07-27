namespace MUEats.Identity.Application.Dtos;

public sealed record RoleDto
{
    public Guid Id { get; init; }
    
    public string Name { get; init; }

    public List<string> Requirements { get; init; } = [];
}