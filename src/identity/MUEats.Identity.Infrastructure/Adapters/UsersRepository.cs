using Microsoft.EntityFrameworkCore;
using MUEats.Identity.Application.Dtos;
using MUEats.Identity.Application.Ports;
using MUEats.Identity.Application.Queries;
using MUEats.Identity.Core.Domain.User;
using MUEats.Identity.Infrastructure.Persistence.DbContexts;

namespace MUEats.Identity.Infrastructure.Adapters;

public class UsersRepository : IUsersRepository
{
    private readonly IdentityDbContext _context;

    public UsersRepository(IdentityDbContext context)
    {
        _context = context;
    }

    public Task AddAsync(User user, CancellationToken ct)
    {
        return _context.Users.AddAsync(user, ct)
            .AsTask();
    }

    public Task<User?> GetByIdAsync(Guid id, CancellationToken ct)
    {
        return _context.Users.
            Include(u => u.Attributes)
            .FirstOrDefaultAsync(x => x.Id == id, ct);
    }

    public Task<User?> GetByEmailAsync(string email, CancellationToken ct)
    {
        return _context.Users.AsNoTracking()
            .Include(u => u.Attributes)
            .FirstOrDefaultAsync(x => x.EmailAddress.Value == email, ct);
    }
    
    public async Task<List<UserDto>> GetUsersAsync(GetUsersQuery query, CancellationToken ct)
    {
        var targetRoleId = await _context.Roles
            .AsNoTracking()
            .Where(x => x.RoleName == query.RoleName)
            .Select(r => r.Id)
            .FirstOrDefaultAsync(ct);

        if (targetRoleId == Guid.Empty)
        {
            return [];
        }
        
        var users = await _context.Users
            .AsNoTracking() 
            .Where(x => x.RoleIds.Contains(targetRoleId))
            .OrderBy(u => u.LastName)
            .ThenBy(u => u.Id) 
            .Skip((query.Page - 1) * query.PageSize)
            .Take(query.PageSize)
            .Select(x => new
            {
                x.Id,
                Email = x.EmailAddress.Value,
                x.FirstName,
                x.LastName,
                x.RoleIds,
                Attributes = x.Attributes.Select(y => new AttributeDto
                {
                    Key = y.Key,
                    Value = y.Value
                }).ToList()
            })
            .ToListAsync(ct);

        if (users.Count == 0)
        {
            return [];
        }

        var currentPageRoleIds = users
            .SelectMany(u => u.RoleIds)
            .Distinct()
            .ToList();

        var rolesMap = await _context.Roles
            .AsNoTracking()
            .Where(r => currentPageRoleIds.Contains(r.Id))
            .ToDictionaryAsync(r => r.Id, r => r.RoleName, ct);

        return users.Select(u => new UserDto
        {
            Id = u.Id,
            Email = u.Email,
            FirstName = u.FirstName,
            LastName = u.LastName,
            Attributes = u.Attributes,
            Roles = u.RoleIds
                .Where(roleId => rolesMap.ContainsKey(roleId))
                .Select(roleId => new RoleDto
                {
                    Name = rolesMap[roleId]
                })
                .ToList()
        }).ToList();
    }
    
    public Task<bool> AnyAsync(string email, CancellationToken ct)
    {
        return _context.Users.AnyAsync(x => x.EmailAddress.Value == email, ct);
    }
}