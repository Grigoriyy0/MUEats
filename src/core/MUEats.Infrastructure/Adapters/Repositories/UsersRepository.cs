using Microsoft.EntityFrameworkCore;
using MUEats.Application.Ports;
using MUEats.Core.Domain.User;
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
        return context.Users.FirstOrDefaultAsync(x => x.Id == id, ct);
    }

    public Task<User?> GetByEmailAsync(string email, CancellationToken ct)
    {
        return context.Users.AsNoTracking()
            .FirstOrDefaultAsync(x => x.Email == email, ct);
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