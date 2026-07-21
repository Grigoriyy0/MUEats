using MUEats.Identity.Application.Dtos;
using MUEats.Identity.Application.Queries;
using MUEats.Identity.Core.Domain.User;

namespace MUEats.Identity.Application.Ports;

public interface IUsersRepository
{
    Task AddAsync(User user, CancellationToken ct);
    
    Task<User?> GetByIdAsync(Guid id, CancellationToken ct);

    Task<User?> GetByEmailAsync(string email, CancellationToken ct);
    
    Task<List<UserDto>> GetUsersAsync(GetUsersQuery query, CancellationToken ct);

    Task<bool> AnyAsync(string email, CancellationToken ct);
}