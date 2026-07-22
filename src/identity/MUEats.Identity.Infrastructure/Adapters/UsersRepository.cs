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
    
    public Task<List<UserDto>> GetUsersAsync(GetUsersQuery query, CancellationToken ct)
    {
        throw new NotImplementedException();
    }
    
    public Task<bool> AnyAsync(string email, CancellationToken ct)
    {
        return _context.Users.AnyAsync(x => x.EmailAddress.Value == email, ct);
    }
}