namespace MUEats.Application.Interfaces;

public interface IRolesService
{
    Task CreateAsync(string roleName, CancellationToken ct);

    Task GrantRoleAsync(Guid userId, Guid roleId, CancellationToken ct);
}