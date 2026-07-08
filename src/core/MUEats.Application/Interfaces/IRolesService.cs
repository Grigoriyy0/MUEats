using CSharpFunctionalExtensions;
using MUEats.Application.Dto.Role;
using Primitives;

namespace MUEats.Application.Interfaces;

public interface IRolesService
{
    Task<UnitResult<Error>> CreateAsync(string roleName, CancellationToken ct);

    Task<UnitResult<Error>> GrantRoleAsync(Guid userId, Guid roleId, CancellationToken ct);

    Task<List<RoleDto>> GetRolesAsync(CancellationToken ct);
}