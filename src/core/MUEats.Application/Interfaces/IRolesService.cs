using CSharpFunctionalExtensions;
using MUEats.Application.Dto.Role;
using Primitives;

namespace MUEats.Application.Interfaces;

public interface IRolesService
{
    Task<UnitResult<Error>> CreateAsync(CreateRoleDto dto, CancellationToken ct);

    Task<UnitResult<Error>> GrantRoleAsync(GrantRoleDto dto, CancellationToken ct);

    Task<List<RoleDto>> GetRolesAsync(CancellationToken ct);
}