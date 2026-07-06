using Microsoft.EntityFrameworkCore;
using MUEats.Application.Dto.User;
using MUEats.Application.Ports;
using MUEats.Application.Queries;
using MUEats.Core.Domain.Constants;
using MUEats.Core.Domain.User;
using MUEats.Core.Domain.User.Entities;
using MUEats.Infrastructure.Persistence;

namespace MUEats.Infrastructure.Adapters.Repositories;

public class UsersRepository(MueDbContext context) : IUsersRepository
{
    public Task AddAsync(User user, CancellationToken ct)
    {
        return context.Users.AddAsync(user, ct)
            .AsTask();
    }

    public Task<User?> GetByIdAsync(Guid id, CancellationToken ct)
    {
        return context.Users.
            Include(u => u.UserAttributes)
            .FirstOrDefaultAsync(x => x.Id == id, ct);
    }

    public Task<User?> GetByEmailAsync(string email, CancellationToken ct)
    {
        return context.Users.AsNoTracking()
            .Include(u => u.UserAttributes)
            .FirstOrDefaultAsync(x => x.Email == email, ct);
    }
    
    public Task<List<UserDto>> GetUsersAsync(GetUsersQuery query, CancellationToken ct)
    {
        return context.Users.Where(y => y.UserRoles
                .Any(x => x.Role.RoleName == query.RoleName))
            .Select(x => new UserDto
        {
            Id = x.Id,
            Email = x.Email,
            FirstName = x.FirstName,
            LastName = x.LastName
        })
        .Skip((query.Page - 1) * query.PageSize)
        .Take(query.PageSize)
            .ToListAsync(ct);
    }
    public Task<bool> AnyAsync(string email, CancellationToken ct)
    {
        return context.Users.AnyAsync(x => x.Email == email, ct);
    }
}