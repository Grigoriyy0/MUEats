using Microsoft.EntityFrameworkCore;
using MUEats.Application.Ports;
using MUEats.Core.Domain.User.Entities;
using MUEats.Infrastructure.Persistence;

namespace MUEats.Infrastructure.Adapters.Repositories;

public class RolesRepository : IRolesRepository
{
    private readonly MueDbContext _context;

    public RolesRepository(MueDbContext context)
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
        return _context.Roles.FirstOrDefaultAsync(x => x.Id == roleId, ct);
    }

    public Task<bool> AnyAsync(string roleName, CancellationToken ct)
    {
        return _context.Roles.AnyAsync(x => x.RoleName == roleName, ct);
    }
}