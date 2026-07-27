using MUEats.Identity.Application.Dtos;
using MUEats.Identity.Core.Domain.Role;

namespace MUEats.Identity.Application.Ports;

public interface IRolesRepository
{
    Task AddAsync(Role role, CancellationToken ct);

    Task<Role?> GetByIdAsync(Guid roleId, CancellationToken ct);

    Task<bool> AnyAsync(string roleName, CancellationToken ct);

    Task<List<RoleDto>> GetAllDtoAsync(CancellationToken ct);

    Task<List<Role>> GetRolesByIdsAsync(List<Guid> roleIds, CancellationToken ct);
}