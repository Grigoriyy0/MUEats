namespace MUEats.Identity.Application.Dtos;

public sealed record CreateRoleDto
{
    public string RoleName { get; init; }

    public List<string>? RequiredAttributes { get; init; } = [];
}