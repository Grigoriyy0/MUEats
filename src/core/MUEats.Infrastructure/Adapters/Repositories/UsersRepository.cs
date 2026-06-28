using Microsoft.EntityFrameworkCore;
using MUEats.Application.Dto.User;
using MUEats.Application.Ports;
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

    //todo fix
    public Task<List<ManagerDto>> GetManagersAsync(CancellationToken ct)
    {
        return context.Users.Where(x => x.UserRoles.
                Any(ur => ur.Role.RoleName == RoleConstants.RoleNames.RestaurantOwner))
            .Select(y => new ManagerDto
            {
                Id = y.Id,
                Email = y.Email,
                UserName = y.Username,
                FirstName = y.FirstName,
                LastName = y.LastName,
                RestaurantId = y.UserAttributes.FirstOrDefault(z => z.Key == "restaurant_id").Value,
            }).ToListAsync(ct);
    }

    public Task DeleteAsync(User user, CancellationToken ct)
    {
        context.Users.Remove(user);
        return Task.CompletedTask;
    }

    public Task UpdateAsync(User user, CancellationToken ct)
    {
        context.Users.Update(user);
        return Task.CompletedTask;
    }

    public Task<bool> AnyAsync(Guid id, CancellationToken ct)
    {
        return context.Users.AnyAsync(x => x.Id == id, ct);
    }

    public Task<bool> AnyAsync(string email, CancellationToken ct)
    {
        return context.Users.AnyAsync(x => x.Email == email, ct);
    }
}