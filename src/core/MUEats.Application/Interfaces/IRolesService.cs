namespace MUEats.Application.Interfaces;

public interface IRolesService
{
    Task CreateAsync(string roleName, CancellationToken ct);
}