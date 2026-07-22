using Microsoft.EntityFrameworkCore;
using MUEats.Identity.Application.Dtos;
using MUEats.Identity.Application.Ports;
using MUEats.Identity.Core.Domain.Role;
using MUEats.Identity.Infrastructure.Persistence.DbContexts;

namespace MUEats.Identity.Infrastructure.Adapters;

public class RolesRepository : IRolesRepository
{
    private readonly IdentityDbContext _context;

    public RolesRepository(IdentityDbContext context)
    {
        _context = context;
    }

    public Task AddAsync(Role role, CancellationToken ct)
    {
        return _context.Roles.AddAsync(role, ct)
            .AsTask();
    }

    public Task<Role?> GetByIdAsync(Guid roleId, CancellationToken ct)
    {
        return _context.Roles
            .Include(x => x.Requirements)
            .FirstOrDefaultAsync(x => x.Id == roleId, ct);
    }

    public Task<bool> AnyAsync(string roleName, CancellationToken ct)
    {
        return _context.Roles.AnyAsync(x => x.RoleName == roleName, ct);
    }

    public Task<List<RoleDto>> GetAllDtoAsync(CancellationToken ct)
    {
        return _context.Roles.Include(x => x.Requirements)
            .Select(x => new RoleDto
            {
                Id = x.Id,
                Name = x.RoleName,
                Requirements = x.Requirements.Select(y => y.ValueName)
                    .ToList()
            }).ToListAsync(ct);
    }

    public Task<List<Role>> GetRolesByIdsAsync(List<Guid> roleIds, CancellationToken ct)
    {
        throw new NotImplementedException();
    }
}