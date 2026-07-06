using MUEats.Application.Dto.Role;
using MUEats.Core.Domain.User.Entities;

namespace MUEats.Application.Ports;

public interface IRolesRepository
{
    Task AddAsync(Role role, CancellationToken ct);

    Task<Role?> GetByIdAsync(Guid roleId, CancellationToken ct);

    Task<bool> AnyAsync(string roleName, CancellationToken ct);

    Task<List<RoleDto>> GetAllDtoAsync(CancellationToken ct);
}