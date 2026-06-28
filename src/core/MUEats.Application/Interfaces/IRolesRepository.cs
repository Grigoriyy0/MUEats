using MUEats.Core.Domain.User.Entities;

namespace MUEats.Application.Interfaces;

public interface IRolesRepository
{
    Task AddAsync(Role role, CancellationToken ct);

    Task<Role?> GetByIdAsync(Guid roleId, CancellationToken ct);

    Task<bool> AnyAsync(string roleName, CancellationToken ct);
}